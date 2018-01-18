using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit{

	public delegate	void OnPositionChangedHandler(Camera cam);

	public	enum	ECameraPosition {
		PositionUndef = -1,
		PositionLeft,
		PositionRight,
		PositionUp,
		PositionDown,
		PositionFar,
		PositionNear,
		PositionBird
	}
    public abstract class Camera {
        protected OnPositionChangedHandler m_OnPositionChanged = null;
        public OnPositionChangedHandler OnPositionChanged {
            get {
                return m_OnPositionChanged;
            }
            set {
                m_OnPositionChanged = value;
            }
        }
        protected float m_angle=0.785f;
        public float Angle {
            get { return this.m_angle; }
            set {
                if(value < Math.PI/180) return;
                if(value >= Math.PI) return;
                this.m_angle = value;
            }
        }

        public abstract void TargetTo(ref Vector3 pt);
        public abstract bool Park(ArrayList dxobj, ECameraPosition position, bool center);
        public abstract Vector3 GetLightVector();
        public abstract void ProcessMouseMove(S3DirectX scene,EAction action,ETarget tg,Point pt,int DX,int DY,OnAction proc);
        public virtual CVTPoint GetCVTPoint(bool force) {
            return null; 
        }
        public abstract Microsoft.DirectX.Matrix GetViewMatrix();
        public abstract void GetDynamicZRange(float zfar_ratio,out float RenderZnear,out float RenderZfar);

    }
}
