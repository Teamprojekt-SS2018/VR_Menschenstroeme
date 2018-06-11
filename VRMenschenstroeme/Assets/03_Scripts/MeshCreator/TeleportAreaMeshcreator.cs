//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: An area that the player can teleport to
//
//=============================================================================

using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class TeleportAreaMeshcreator : TeleportMarkerBase
    {
        //Public properties
        public Bounds meshBounds { get; private set; }

        //Private data
        public MeshRenderer areaMesh;
        private int tintColorId = 0;
        private Color visibleTintColor = Color.clear;
        private Color highlightedTintColor = Color.clear;
        private Color lockedTintColor = Color.clear;
        private bool highlighted = false;


        //-------------------------------------------------
        public void Init()
        {

            tintColorId = Shader.PropertyToID("_TintColor");

            CalculateBounds();

            visibleTintColor = Teleport.instance.areaVisibleMaterial.GetColor(tintColorId);
            highlightedTintColor = Teleport.instance.areaHighlightedMaterial.GetColor(tintColorId);
            lockedTintColor = Teleport.instance.areaLockedMaterial.GetColor(tintColorId);
        }


        //-------------------------------------------------
        public override bool ShouldActivate(Vector3 playerPosition)
        {
            return true;
        }


        //-------------------------------------------------
        public override bool ShouldMovePlayer()
        {
            return true;
        }


        //-------------------------------------------------
        public override void Highlight(bool highlight)
        {
            if (!locked)
            {
                highlighted = highlight;

                if (highlight)
                {
                    areaMesh.material = Teleport.instance.areaHighlightedMaterial;
                }
                else
                {
                    areaMesh.material = Teleport.instance.areaVisibleMaterial;
                }
            }
        }


        //-------------------------------------------------
        public override void SetAlpha(float tintAlpha, float alphaPercent)
        {
            Color tintedColor = GetTintColor();
            tintedColor.a *= alphaPercent;
            areaMesh.material.SetColor(tintColorId, tintedColor);
        }


        //-------------------------------------------------
        public override void UpdateVisuals()
        {
            if (locked)
            {
                areaMesh.material = Teleport.instance.areaLockedMaterial;
            }
            else
            {
                areaMesh.material = Teleport.instance.areaVisibleMaterial;
            }
        }


        //-------------------------------------------------
        public void UpdateVisualsInEditor()
        {
            areaMesh = GetComponent<MeshRenderer>();

            if (locked)
            {
                areaMesh.sharedMaterial = Teleport.instance.areaLockedMaterial;
            }
            else
            {
                areaMesh.sharedMaterial = Teleport.instance.areaVisibleMaterial;
            }
        }


        //-------------------------------------------------
        private bool CalculateBounds()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                return false;
            }

            Mesh mesh = meshFilter.mesh;
            if (mesh == null)
            {
                return false;
            }
            mesh.RecalculateBounds();

            meshBounds = mesh.bounds;
            return true;
        }


        //-------------------------------------------------
        private Color GetTintColor()
        {
            if (locked)
            {
                return lockedTintColor;
            }
            else
            {
                if (highlighted)
                {
                    return highlightedTintColor;
                }
                else
                {
                    return visibleTintColor;
                }
            }
        }
    }
}
