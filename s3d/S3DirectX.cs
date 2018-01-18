using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
	public enum FillMode{
		POINT = Microsoft.DirectX.Direct3D.FillMode.Point,
		WIREFRAME = Microsoft.DirectX.Direct3D.FillMode.WireFrame,
		SOLID = Microsoft.DirectX.Direct3D.FillMode.Solid
	}
	public enum BFCullMode {
		None = Microsoft.DirectX.Direct3D.Cull.None, // Do not cull back faces
		CW = Microsoft.DirectX.Direct3D.Cull.Clockwise, // Cull back faces with clockwise vertices
		CCW = Microsoft.DirectX.Direct3D.Cull.CounterClockwise // Cull back faces with counterclockwise vertices
	}

	public enum ETarget{
		TargetCamera=0,
		TargetObject=1,
		TargetGrip=2
	}
	public enum EAction{
		Undef=-1,
		ActionRotate=0,  // (x,y)
		ActionPan=1,     // (x,y)
		ActionZoom=2,    // (d)
		ActionSlide = 3,
		ActionXYSlide = 4,
		ActionMoveTo = 5,
		ActionZRotate=9,
        ActionClick=10,
        ActionAltClick=12,
        ActionCtrlClick=14,
        ActionShiftClick=16,
        ActionCtrlShiftClick=18,
        ActionCtrlAltClick=20,
        ActionRClick=11,
        ActionAltRClick=13,
        ActionCtrlRClick=15,
		ActionShiftRClick=17,
		ActionCtrlShiftRClick=19,
        ActionLRClick=30
    };

	public delegate void OnAction(EAction action, double x,double y,double ang); // rotate, slide, click, moveto (for camera) 

    public delegate Vector3 CVTPoint(ref Vector3 p); // point => point

    public class KeyboardState: IKeysProvider {
        public bool m_qUp;
        public bool m_qDown;
        public bool m_qLeft;
        public bool m_qRight;
        public bool m_qPgUp;
        public bool m_qPgDown;
        public bool m_qShift;
        public bool m_qCtrl;
        public bool m_qAlt;
        public bool m_qLeftMouse;
        public bool m_qRightMouse;
        public bool m_qX;
        public bool m_qY;
        public bool m_qZ;
        public bool m_qA;

        public bool qLeftMouse {
            set { m_qLeftMouse = value; }
            get { return m_qLeftMouse; }
        }

        public bool qRightMouse {
            set { m_qRightMouse = value; }
            get { return m_qRightMouse; }
        }

        public bool qShift {
            set { m_qShift = value; }
            get { return m_qShift; }
        }

        public bool qCtrl {
            set { m_qCtrl = value; }
            get { return m_qCtrl; }
        }

        public bool qAlt {
            set { m_qAlt = value; }
            get { return m_qAlt; }
        }

        public bool qA {
            set { m_qA = value; }
            get { return m_qA; }
        }

        public bool qX {
            set { m_qX = value; }
            get { return m_qX; }
        }

        public bool qY {
            set { m_qY = value; }
            get { return m_qY; }
        }

        public bool qZ {
            set { m_qZ = value; }
            get { return m_qZ; }
        }

        public KeyboardState(MouseEventArgs e) {
            m_qLeftMouse=e.Button == MouseButtons.Left;
            m_qRightMouse=e.Button == MouseButtons.Right;
            m_qShift=(Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != 0;
            m_qCtrl=(Control.ModifierKeys & System.Windows.Forms.Keys.Control) != 0;
            m_qAlt=(Control.ModifierKeys & System.Windows.Forms.Keys.Alt) != 0;
            m_qX = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_X) & 0x8000) != 0);
            m_qY = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_Y) & 0x8000) != 0);
            m_qZ = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_Z) & 0x8000) != 0);
            m_qUp = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_UP) & 0x8000) != 0);
            m_qDown = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_DOWN) & 0x8000) != 0);
            m_qLeft = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_LEFT) & 0x8000) != 0);
            m_qRight = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_RIGHT) & 0x8000) != 0);
            m_qPgUp = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_PGUP) & 0x8000) != 0);
            m_qPgDown = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_PGDOWN) & 0x8000) != 0);
            m_qA = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_A) & 0x8000) != 0);
        }

        public KeyboardState() {
            m_qLeftMouse = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_LBUTTON) & 0x8000) != 0);
            m_qRightMouse = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_RBUTTON) & 0x8000) != 0);
            m_qUp = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_UP) & 0x8000) != 0);
            m_qDown = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_DOWN) & 0x8000) != 0);
            m_qLeft = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_LEFT) & 0x8000) != 0);
            m_qRight = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_RIGHT) & 0x8000) != 0);
            m_qPgUp = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_PGUP) & 0x8000) != 0);
            m_qPgDown = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_PGDOWN) & 0x8000) != 0);
            m_qShift = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_SHIFT) & 0x8000) != 0);
            m_qCtrl = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_CONTROL) & 0x8000) != 0);
            m_qAlt = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_ALT) & 0x8000) != 0);
            m_qX = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_X) & 0x8000) != 0);
            m_qY = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_Y) & 0x8000) != 0);
            m_qZ = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_Z) & 0x8000) != 0);
            m_qA = ((S3DirectX.GetAsyncKeyState((int)VK_CODES.VK_A) & 0x8000) != 0);
        }

        public bool Eq(IKeysProvider m) {
            return m_qLeftMouse == m.qLeftMouse && m_qA == m.qA
				&&	m_qShift == m.qShift && m_qCtrl == m.qCtrl && m_qAlt==m.qAlt
				&&	m_qX == m.qX && m_qY == m.qY
				&&	m_qZ == m.qZ;
        }
    }

	enum VK_CODES {
		VK_UP = 0x26,
		VK_DOWN = 0x28,
		VK_LEFT = 0x25,
		VK_RIGHT = 0x27,
		VK_SHIFT = 0x10,
		VK_PGUP = 0x21,
		VK_PGDOWN = 0x22,
		VK_X = 0x58,
		VK_Y = 0x59,
		VK_Z = 0x5A,
		VK_CONTROL = 0x11,
		VK_LBUTTON = 0x01,
		VK_RBUTTON = 0x02,
		VK_ALT = 0x12,
		VK_A = 0x41
	}

	public struct MYVERTEX {
        public Vector3 p;
        public Vector3 n;
        public int dc; //diffuse color
        public MYVERTEX(int dummy) { p = new Vector3(); n=new Vector3(); dc = 0; }
		public static readonly VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | VertexFormats.Diffuse;
	}

	public class S3DirectX : DxBase {
		Camera m_Camera = null;
        CameraLight m_Light = null;
		Color m_bgColor = Color.FromArgb(255,0,0,0);

		Point m_lastPoint = new Point(0,0);
		EAction m_lastAction=EAction.Undef;

        FillMode m_RenderFillMode=FillMode.SOLID; //.WIREFRAME;
		//++ vsv 08/07
		BFCullMode m_BFCullMode = BFCullMode.None; // test, was: BFCullMode.CCW;

		public Device DxDevice{ get{ return m_pd3dDevice; }}
		//




		public Color BGColor{
			get{ return this.m_bgColor; }
			set {
				this.m_bgColor = value;
				this.SceneChanged = true;
			}
		}

		public FillMode RenderFillMode{
			get{ return m_RenderFillMode; }
			set {
				m_RenderFillMode=value;
				if(m_pd3dDevice != null) {
					m_pd3dDevice.RenderState.FillMode = (Microsoft.DirectX.Direct3D.FillMode)value;
					this.SceneChanged = true;
				}
			}
		}

		public BFCullMode BackFaceCullMode {
			get { 
				return m_BFCullMode;
			}
			set {
				m_BFCullMode = value;
				if( m_pd3dDevice != null )
					m_pd3dDevice.RenderState.CullMode = (Cull)value;
				this.SceneChanged = true;
			}
		}

		public S3DirectX() {
			m_bUseDepthBuffer = true;

//            Camera _cam = new CCamera();
            Camera _cam = new S4Camera(4,2);
            m_Camera=_cam;
			m_Light=new CameraLight(_cam);
		}

		public void ClearDisplay(){
			try{
				// clear back buffer
				m_pd3dDevice.Clear(
					ClearFlags.Target | ClearFlags.ZBuffer,
					m_bgColor,
					1.0F,
					0);
			}catch(Exception e){
				DxBase.ReportError(e);
			}
		}

		public bool InitDirect3D(UserControl hwnd) {
			bool res = this.Create(hwnd);
			if(res)
				ClearDisplay();
			return res;
		}

		public Camera Camera {
			get {
                if(m_Camera!=null) return m_Camera;
				Debug.Fail("No active camera!!!");
				return null;
			}
		}

        public CameraLight Light {
			get {
				return m_Light;
			}
		}

		static public BaseMesh Create3DMesh(Device d3dDevice,int num_elems,MYVERTEX[] vertices,int num_index,ushort []indices){
			Mesh pMesh = new Mesh(
				num_index/3,	// number of faces
				num_elems,		// number of vertices
				MeshFlags.Managed|MeshFlags.SystemMemory,  //options
				MYVERTEX.Format,
				d3dDevice
				);
			// copy our vertices

			MYVERTEX [] v = (MYVERTEX [])pMesh.LockVertexBuffer(typeof(MYVERTEX), LockFlags.NoSystemLock, num_elems);
			if(v == null){
				MessageBox.Show("Can't lock vertex buffer");
				return null;
			}
			int i;
			for(i=0; i<num_elems; i++) {
				v[i] = vertices[i];
			}
			pMesh.UnlockVertexBuffer();
			// Setup our index buffer
			ushort [] pIndex =(ushort [])pMesh.LockIndexBuffer(typeof(ushort), LockFlags.NoSystemLock, num_index);
			if(pIndex == null) {
				MessageBox.Show("Can't lock index buffer");
				return null;
			}
			for(i=0; i<num_index; i++) {
				pIndex[i] = indices[i];
			}
			pMesh.UnlockIndexBuffer();
			return (BaseMesh)pMesh;
		}

		public void RePaint(){
			try {
				m_pd3dDevice.Present();
			}
			catch {
				SceneChanged = true;
			}
		}
		public void		SetupObjectMaterial(Color color){
			// Set up a material. The material here just has the diffuse and ambient
			// colors set to yellow. Note that only one material can be used at a time.
			Material mtrl = new Material();
			mtrl.Diffuse = color;
				mtrl.Ambient = color;
			m_pd3dDevice.Material = mtrl;
		}

		public void		SetupWorldMatrix(Microsoft.DirectX.Matrix  matr){
			m_pd3dDevice.SetTransform(TransformType.World, matr);
		}

		[DllImport("user32.dll")]
		public static extern short	GetAsyncKeyState(int key);

		public void ProcessMouseMove(MouseEventArgs e, ETarget tg, OnAction proc) {
			ProcessMouseMove(e, tg, new KeyboardState(e), proc);
		}

		public void ProcessMouseMove(MouseEventArgs e, ETarget tg, IKeysProvider keys, OnAction proc) {
			int CX = e.X;
			int CY = e.Y;
			Point _thisPoint = new Point(CX, CY);

			if(!keys.qLeftMouse && !keys.qRightMouse){
				m_lastAction=EAction.Undef; return;
			}

			EAction _thisAction=EAction.ActionRotate;
			if(tg==ETarget.TargetGrip){
				_thisAction=EAction.ActionPan;
			}else{
				// none => rotate
				// ctrl => moveto/FLength
				// shift => pan
				// Right => Z-slide/z-rotate

                if(keys.qRightMouse) {
                    if(keys.qShift) _thisAction=EAction.ActionXYSlide;
                    else if(keys.qCtrl) _thisAction=EAction.ActionZoom;
                    else _thisAction=EAction.ActionSlide;
                } else if(keys.qCtrl) _thisAction=EAction.ActionMoveTo;
                else if(keys.qShift) _thisAction=EAction.ActionPan;
                else _thisAction=EAction.ActionRotate;
			}

			if(m_lastAction!=EAction.Undef){
				lock(this) {
					int	_DX=CX-m_lastPoint.X;
					int _DY=CY-m_lastPoint.Y;

					Camera.ProcessMouseMove(this,_thisAction,tg,_thisPoint,_DX,_DY,proc);
				}
			}
			m_lastPoint = _thisPoint;
			m_lastAction = _thisAction;
			SceneChanged = true;		
		}

		public void ProcessMouseClick(MouseEventArgs e,ETarget tg, OnAction proc){
			int x=e.X,y=e.Y;
			Point pt=new Point(x,y);
			IKeysProvider kk=new KeyboardState(e);
            EAction act;
			bool ert=(e.Button==MouseButtons.Right);
            if(kk.qCtrl) {
                if(kk.qAlt) act=EAction.ActionCtrlAltClick;
                else if(kk.qShift) act=ert ? EAction.ActionCtrlShiftRClick : EAction.ActionCtrlShiftClick;
                else act=ert ? EAction.ActionCtrlRClick : EAction.ActionCtrlClick;
            } else if(kk.qShift) act=ert ? EAction.ActionShiftRClick : EAction.ActionShiftClick;
            else if(kk.qAlt) act=ert ? EAction.ActionAltRClick : EAction.ActionAltClick;
            else act=ert ? EAction.ActionRClick : EAction.ActionClick;
			Camera.ProcessMouseMove(this,act,tg,pt,0,0,proc);
		}

		public ushort width;
		public ushort height;


		Microsoft.DirectX.Matrix	matProj;
		public	Microsoft.DirectX.Matrix	matView;
		Viewport	Viewport = new Viewport();

		public float zfar_ratio =10000;
		public bool	 dynamic_znear = true;
		public float znear = 4;
		public float zfar = 1e5f;

		public float RenderZnear=4;
		public float RenderZfar=1e5f;



        public void BuildMatrices(Device pID3DDevice) {
            matView=Camera.GetViewMatrix();

			pID3DDevice.Transform.View = matView;

			if (dynamic_znear) {
                Camera.GetDynamicZRange(zfar_ratio,out RenderZnear,out RenderZfar);
			}
			else {
				RenderZnear = znear;
				RenderZfar = zfar;
			}

			float aspect=((float)width)/((float)height);
#if false
            Microsoft.DirectX.Matrix matrix = Microsoft.DirectX.Matrix.PerspectiveFovLH(Camera.Angle,aspect,RenderZnear,RenderZfar);
#else
            float ang=Camera.Angle;
            Microsoft.DirectX.Matrix matrix = Microsoft.DirectX.Matrix.OrthoLH(2*ang*aspect,2*ang,-100,100);

#endif
			pID3DDevice.Transform.Projection = matrix;
			matProj = matrix;
			Viewport.X = 0;
			Viewport.Y = 0;
			Viewport.MinZ = 0;
			Viewport.MaxZ = 1;
			Viewport.Width = width;
			Viewport.Height = height;
			pID3DDevice.Viewport = Viewport;
		}

		public Vector3	ProjectPoint(Vector3 pt, Microsoft.DirectX.Matrix world) {
			return	Vector3.Project(pt, 
				Viewport,
				matProj,
				matView,
				world);
		}

		public bool DirAndOrig(int x, int y,out double vx,out double vy) {
			vx = ( ( ( 2.0f * x ) / width  ) - 1 ) / matProj.M11;
			vy = -( ( ( 2.0f * y ) / height ) - 1 ) / matProj.M22;
			return true;
		}

        bool qRenderOn=false;
		// Overridable functions for the 3D scene created by the app
		override public int	Render() {
            if(qRenderOn) return S_OK;
            qRenderOn=true;
			int hr = S_OK;
			ArrayList _dxobj = ((IDXControl)m_hWnd).DXObjects;

			// start drawing
			m_pd3dDevice.BeginScene();

			ClearDisplay();

			this.BuildMatrices(m_pd3dDevice);
			if(m_Light!=null){
                m_Light.Setup(m_pd3dDevice, 0);
			}
			// Turn on some ambient light.
			m_pd3dDevice.RenderState.Ambient = Color.FromArgb(0 , 0x20, 0x20, 0x20 );
			foreach (IDXObject _obj in _dxobj) {
                m_pd3dDevice.RenderState.Lighting = _obj.NeedLightning;
				_obj.Render(this);
			}
			m_pd3dDevice.EndScene();
            qRenderOn=false;
			// flip back buffer to front
			return hr;
		}

		override public int AfterResize(int w, int h) {
			height = (ushort)h;
			width = (ushort)w;
			SceneChanged = true;
			return S_OK;
		}
		public override int RestoreDeviceObjects(){
			// we do our own coloring, so disable lighting
			m_pd3dDevice.RenderState.Lighting = false;
			// enable the z-buffer
			m_pd3dDevice.RenderState.ZBufferEnable = true;

			m_pd3dDevice.RenderState.PointScaleEnable = false;
			float psz = 1.0f;
			m_pd3dDevice.RenderState.PointSize = psz;
			m_pd3dDevice.RenderState.FillMode = (Microsoft.DirectX.Direct3D.FillMode)m_RenderFillMode;
			m_pd3dDevice.RenderState.CullMode = (Cull)m_BFCullMode;

			m_pd3dDevice.RenderState.AlphaBlendEnable = true;
			m_pd3dDevice.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
			m_pd3dDevice.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;

			m_pd3dDevice.RenderState.SpecularMaterialSource=ColorSource.Color1;
			m_pd3dDevice.RenderState.SpecularEnable=true;


			return S_OK;
		}
		override public int FinalCleanup(){
			return S_OK;
		}
		public override int InvalidateDeviceObjects(){
			return DxBase.S_OK;
		}
	}

}

