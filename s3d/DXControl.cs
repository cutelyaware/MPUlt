using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Timers;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace _3dedit
{
	/// <summary>
	/// Summary description for DXControl.
	/// </summary>
	public class DXControl : System.Windows.Forms.UserControl, IDXControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Thread	m_KeyBoardThread = null;
		private ManualResetEvent	m_KeyBoardThreadStop = new ManualResetEvent(false);
		private System.ComponentModel.Container components = null;

		private EventHandler m_hidl;
		private System.Timers.Timer m_timer = new System.Timers.Timer();
		private S3DirectX m_DDeviceX = new S3DirectX();
		ArrayList	m_DxObjects = new ArrayList();


		public DXControl(){
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			m_hidl = new EventHandler(this.IdleHandlerFun);
			Application.Idle += m_hidl;
			m_timer.Elapsed += new ElapsedEventHandler(TimerEventProcessor);
			m_timer.Interval = 200;
			//			ForceInit();
  

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (m_KeyBoardThread != null) {
					m_KeyBoardThreadStop.Set();
					m_KeyBoardThread.Join(1000);
				}
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	
	
		protected override void OnLoad(EventArgs e) {
			base.OnLoad (e);
			this.ForceInit();
		}
		protected override void OnSizeChanged(System.EventArgs e) {
			ResizeEvt(e);
			base.OnSizeChanged(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(m_DDeviceXInited && m_DDeviceX.IsReady) {
				m_DDeviceX.RePaint();
				//m_DDeviceX.Render3DEnvironment();
			}
			base.OnPaint(e);
		}
		public void ProcessMouse(MouseEventArgs e,ETarget tg,OnAction act){
			if(!m_DDeviceXInited)
				return;
			m_DDeviceX.ProcessMouseMove(e,tg,act);
		}
		public void ProcessMouse(MouseEventArgs e, ETarget tg, IKeysProvider keys, OnAction act) {
			if(!m_DDeviceXInited)
				return;
			m_DDeviceX.ProcessMouseMove(e, tg, keys, act);
		}
		public void ProcessPick(MouseEventArgs e, ETarget tg,OnAction proc){
			if(!m_DDeviceXInited)
				return;
			m_DDeviceX.ProcessMouseClick(e,tg,proc);
		}
		public void SetPOV(ref Vector3 pt){
			if(!m_DDeviceXInited) return;
			m_DDeviceX.Camera.TargetTo(ref pt);
		}

		private void ResizeEvt(EventArgs e) {
			if(m_DDeviceXInited)
				m_DDeviceX.Resize = true;
		}
		private bool UpdateHandler() {
			bool ret = false;
			if(!m_DDeviceXInited)
				return ret;
			lock(this) {
				if	(	(m_DDeviceX.IsReady)
					&&	(this.ParentForm.Visible)
					&&	(this.ParentForm.WindowState != System.Windows.Forms.FormWindowState.Minimized)) {
					if (m_DDeviceX.Resize) {
						m_DDeviceX.Resize = false;
						m_DDeviceX.Resize3DEnvironment();
					}
					if (m_DDeviceX.SceneChanged) {
						m_DDeviceX.SceneChanged = false;
						m_DDeviceX.Render3DEnvironment();
					}
				}
			}
			return ret;
		}


		public	bool ForceUpdate() {
			SetSceneChanged();
			return UpdateHandler();
		}

		private void IdleHandlerFun(object source, EventArgs e) {
            if(m_timer==null) return;
			if(m_timer.Enabled)
				return;
			bool ret = UpdateHandler();
			if(ret) {
				m_timer.Start();
			}
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			Application.Idle -= m_hidl;
			base.OnHandleDestroyed(e);
		}
		private void TimerEventProcessor(object myObject,
			ElapsedEventArgs myEventArgs) {
			if(m_timer==null || !m_timer.Enabled)
				return;
			((System.Timers.Timer)myObject).Stop();
			Application.DoEvents();
			bool ret = UpdateHandler();
			if(ret)
				m_timer.Start();
		}



		bool	m_DDeviceXInited = false;
		public bool ForceInit() {
			if(!m_DDeviceXInited) {
				if( !m_DDeviceX.InitDirect3D( this))
					return false;
				m_DDeviceX.Resize3DEnvironment();		
				ThreadStart	_proc = new ThreadStart(this.KeyProc);
				m_KeyBoardThread = new Thread(_proc);
				m_KeyBoardThread.Start();

				m_DDeviceXInited = true;
			}
			return true;
		}

		public	const	int KeyboardRate = 250;
		void KeyProc() {
			while (true) {
//				m_DDeviceX.ProcessKeyboardEvt();
				if (m_KeyBoardThreadStop.WaitOne(1000/KeyboardRate, false))
					break;
			}
		}

		public virtual void SetSceneChanged() {
			if (m_DDeviceXInited)
				m_DDeviceX.SceneChanged = true;
		}

		public virtual S3DirectX Scene {
			get{ return m_DDeviceX; }
		}

		public ArrayList	DXObjects {
			get	{	return m_DxObjects;	}
		}

		public void AddMesh(IDXObject obj){
			DXObjects.Add(obj);
			Scene.SceneChanged=true;
		}

		public void ParkCamera(bool chPos){
			if(chPos) Scene.Camera.Park(DXObjects,ECameraPosition.PositionDown,false);
			else Scene.Camera.Park(DXObjects,ECameraPosition.PositionUndef,false);
			Scene.SceneChanged=true;
		}



		public void RemoveMesh(IDXObject obj){
			DXObjects.Remove(obj);
			Scene.SceneChanged=true;
		}

		public void ClearMeshes(){
			DXObjects.Clear();
			Scene.SceneChanged=true;
		}



		public FillMode FillMode{
			get{
				return m_DDeviceX.RenderFillMode;
			}
			set{
				m_DDeviceX.RenderFillMode=value;
			}
		}


	}
}
