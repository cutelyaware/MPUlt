using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
	/// <summary>
	/// Summary description for Light.
	/// </summary>
	public class Light {
		protected Vector3 m_lightVector;
		protected LightType m_Type = LightType.Directional;
		protected int m_ambient = 150;
		protected int m_diffuse = 150;
		protected int m_specular = 100;

		public int Diffuse {
			get { return m_diffuse; }
			set {	m_diffuse = value;	}
		}

		public int Ambient {
			get { return m_ambient; }
			set {	m_ambient = value;	}
		}

		public int Specular {
			get { return m_specular; }
			set {	m_specular = value;	}
		}

		public Vector3 LightVector {
			get{ return m_lightVector; }
			set{ m_lightVector = value; }
		}

		public void UpdateLightVector(Camera camera) {
            m_lightVector=camera.GetLightVector();
		}

		bool	m_bEnable = true;
		public	bool	Enable {
			get {
				return m_bEnable;
			}
			set {
				m_bEnable = value;
			}
		}

		public void Setup(Device pID3DDevice, int n) {
			if (m_bEnable) {
				// Set up a white, directional light, with an oscillating direction.
				// Note that many lights may be active at a time (but each one slows down
				// the rendering of our scene). However, here we are just using one. Also,
				// we need to set the D3DRS_LIGHTING renderstate to enable lighting
				pID3DDevice.Lights[n].Type       = LightType.Directional;
				//
				pID3DDevice.Lights[n].Diffuse  = Color.FromArgb(m_diffuse, m_diffuse, m_diffuse);
				//
				pID3DDevice.Lights[n].Ambient  = Color.FromArgb(m_ambient, m_ambient, m_ambient);
				//
				pID3DDevice.Lights[n].Specular  = Color.FromArgb(m_specular, m_specular, m_specular);
				//
				pID3DDevice.Lights[n].Direction   = LightVector;
				//
				pID3DDevice.Lights[n].Range       = 1e10f;
				//pID3DDevice.Lights[n].Commit();
			}
			pID3DDevice.Lights[n].Enabled = m_bEnable;
		}
	}

	public	class CameraLight	:	Light {
		Camera	m_Cam = null;

		public Camera Camera {
			get {
				return m_Cam;
			}
		}

		public CameraLight(Camera cam) {
			m_Cam = cam;
			UpdateLightVector(cam);
			cam.OnPositionChanged += new OnPositionChangedHandler(UpdateLightVector);
		}
	}
}
