using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
	public class IDispModesComparer : IComparer {
		int IComparer.Compare(object o1, object o2) {
			DisplayMode p1 = (DisplayMode)o1;
			DisplayMode p2 = (DisplayMode)o2;

			if( p1.Format > p2.Format )   return -1;
			if( p1.Format < p2.Format )   return +1;
			if( p1.Width  < p2.Width )    return -1;
			if( p1.Width  > p2.Width )    return +1;
			if( p1.Height < p2.Height )   return -1;
			if( p1.Height > p2.Height )   return +1;
			return 0;
		}
	}

	public enum DxError{
		D3DAPPERR_NODIRECT3D,
		D3DAPPERR_NOCOMPATIBLEDEVICES,
		D3DAPPERR_NOWINDOWABLEDEVICES,
		D3DAPPERR_NOHARDWAREDEVICE,
		D3DAPPERR_HALNOTCOMPATIBLE,
		D3DAPPERR_NOWINDOWEDHAL,
		D3DAPPERR_NODESKTOPHAL,
		D3DAPPERR_NOHALTHISMODE,
		D3DAPPERR_MEDIANOTFOUND,
		D3DAPPERR_RESIZEFAILED,
		D3DAPPERR_NONZEROREFCOUNT,
		D3DAPPERR_GENERIC
	}
	//-----------------------------------------------------------------------------
	// Name: struct D3DModeInfo
	// Desc: Structure for holding information about a display mode
	//-----------------------------------------------------------------------------
	public class D3DModeInfo {
		public int			Width;      // Screen width in this mode
		public int			Height;     // Screen height in this mode
		public Format		Format;     // Pixel format in this mode
		public CreateFlags	dwBehavior; // Hardware / Software / Mixed vertex processing
		public DepthFormat	DepthStencilFormat; // Which depth/stencil format to use with this mode
		public D3DModeInfo(){
		}
	};
	//-----------------------------------------------------------------------------
	// Name: struct D3DDeviceInfo
	// Desc: Structure for holding information about a Direct3D device, including
	//       a list of modes compatible with this device
	//-----------------------------------------------------------------------------
	public class D3DDeviceInfo {
		// Device data
		public Microsoft.DirectX.Direct3D.DeviceType DeviceType;      // Reference, HAL, etc.
		public DeviceCaps d3dCaps;         // Capabilities of this device
		public string strDesc="";         // Name of this device
		public bool bCanDoWindowed;  // Whether this device can work in windowed mode

		// Modes for this device
		public int dwNumModes=0;
		public D3DModeInfo  []modes=new D3DModeInfo[150];
		public D3DDeviceInfo(){
			for(int i = 0; i < 150; i++) modes[i] = new D3DModeInfo();
		}

		// Current state
		public int dwCurrentMode;
		public bool bWindowed;
		public MultiSampleType MultiSampleTypeWindowed;
		public MultiSampleType MultiSampleTypeFullscreen;
	}
	//-----------------------------------------------------------------------------
	// Name: struct D3DAdapterInfo
	// Desc: Structure for holding information about an adapter, including a list
	//       of devices available on this adapter
	//-----------------------------------------------------------------------------
	public class D3DAdapterInfo {
		// Adapter data
		public int			d3dAdapterIdentifier;
		public DisplayMode	d3ddmDesktop;      // Desktop display mode for this adapter

		// Devices for this adapter
		public int dwNumDevices;
		public D3DDeviceInfo []devices = new D3DDeviceInfo[5];
		// Current state
		public int          dwCurrentDevice;
		public D3DAdapterInfo(){
			for(int i = 0; i < 5; i++) devices[i] = new D3DDeviceInfo();
		}
	}
	public class DxBase	{
		public const int S_OK=0;
		public const int S_FALSE=-1;
		// Internal variables for the state of the app
		protected D3DAdapterInfo []m_Adapters = new D3DAdapterInfo[10];
		protected int m_dwNumAdapters = 0;
		protected int m_dwAdapter = 0;
		protected bool m_bWindowed = false;
		protected bool m_bReady = false;
		protected bool m_bResize = false;
		// Main objects used for creating and rendering the 3D scene
		//		protected DirectX8 m_dx8; // Used to create the Direct3D8
		//		protected Direct3D8 m_pD3D; // Used to create the D3DDevice
		protected Device m_pd3dDevice; // The D3D rendering device
		public PresentParameters m_d3dpp;         // Parameters for CreateDevice/Reset
		protected Caps m_d3dCaps;           // Caps for the device
		protected UserControl m_hWnd;              // The main app window
		protected WCaptureLoss	m_win; //Native window
		protected CreateFlags	m_dwCreateFlags;     // Indicate sw or hw vertex processing
		// Overridable variables for the app
		protected bool m_bUseDepthBuffer = false;   // Whether to autocreate depthbuffer
		protected int m_dwMinDepthBits;    // Minimum number of bits needed in depth buffer
		protected int m_dwMinStencilBits;  // Minimum number of bits needed in stencil buffer
		// Variables for timing
		string m_strDeviceStats;// String to hold D3D device stats
		public DxBase()	{
			//			m_dx8 = null; m_pD3D = null;
			m_pd3dDevice = null;
			m_dwMinDepthBits    = 16;
			m_dwMinStencilBits  = 0;
			for(int i = 0; i < 10; i++) m_Adapters[i] = new D3DAdapterInfo();
		}
		public bool IsReady{
			get{ return m_bReady;  }
		}
		public bool	Resize {
			set{	m_bResize = value;	}
			get{	return m_bResize;	}
		}
		//		bool _RegisterDll(){
		//			try{
		//				Process pr = new Process();
		//				ProcessStartInfo si = new ProcessStartInfo("regsvr32","/s dx8vb.dll");
		//				pr.StartInfo = si;
		//				pr.Start();
		//				while(!	pr.HasExited )
		//					Application.DoEvents();
		//			}catch{
		//				return false;
		//			}
		//			return true;
		//		}
		protected bool Create(UserControl hWnd){
			//			bool first = true;
			m_hWnd = hWnd;
			//init:
			try {
				//				m_dx8 = new DirectX8();
				//				
				//				m_pD3D = m_dx8.Direct3DCreate();
				//				if(m_pD3D == null){
				//					DisplayErrorMsg(DxError.D3DAPPERR_NODIRECT3D);
				//					return false;
				//				}
				// Build a list of Direct3D adapters, modes and devices.
				if(!BuildDeviceList()){
					return false;
				}
				// Initialize the 3D environment for the app
				if(FAILED(Initialize3DEnvironment()))
					return false;
				m_bReady = true;
				if(m_win==null){
					m_win= new WCaptureLoss();
					m_win.AssignHandle(m_hWnd.FindForm().Handle);
					m_win.OnSizeMove += new SizeMoveHandler(OnSizeMove);
				}
				return true;
			}catch(Exception e){
				//
				//				if(first){
				//					first = false;
				//					string sCLSID = "E7FF1300-96A5-11D3-AC85-00C04FC2C602";
				//					if(e.Message.IndexOf(sCLSID) >= 0){
				//						if(_RegisterDll())
				//							goto init;
				//					}
				//				}
				ReportError(e);
				//String2Err(e.Message);
				return false;
			}
		}
		public static void String2Err(string str){
			int idx = str.IndexOf("HRESULT: 0x");
			if(idx < 0){
				MessageBox.Show(str,"Error");
				return;
			}
			str = str.Substring(idx+11);
			idx = str.IndexOf(".");
			str = str.Substring(0,idx);
			int num = (int)uint.Parse(str,System.Globalization.NumberStyles.HexNumber);
			str = NativeMethods.HResult2ErrorText(num);
			if(str!=""){
				MessageBox.Show(str,"Error");
				return;
			}
			//			bool res = DisplayD3DErrMsg((CONST_D3DERR)num);
			//			if(res)
			//				return;
			//			DisplayErrorMsg(DxError.D3DAPPERR_GENERIC);
		}
		bool BuildDeviceList() {
			const int dwNumDeviceTypes = 2;
			string []strDeviceDescs = new string[]{"HAL", "REF"};
			DeviceType []DeviceTypes = new DeviceType[]{
														   DeviceType.Hardware,
														   DeviceType.Reference
													   };
			bool bHALExists = false;
			bool bHALIsWindowedCompatible = false;
			bool bHALIsDesktopCompatible = false;
			bool bHALIsSampleCompatible = false;
			// Loop through all the adapters on the system (usually, there's just one
			// unless more than one graphics card is present).
			int nAdapters = Manager.Adapters.Count;
			for(int iAdapter = 0; iAdapter < nAdapters; iAdapter++) {
				// Fill in adapter info
				D3DAdapterInfo pAdapter  = m_Adapters[m_dwNumAdapters];
				pAdapter.d3dAdapterIdentifier = Manager.Adapters[iAdapter].Adapter;
				pAdapter.d3ddmDesktop = Manager.Adapters[iAdapter].CurrentDisplayMode;
				pAdapter.dwNumDevices    = 0;
				pAdapter.dwCurrentDevice = 0;

				// Enumerate all display modes on this adapter
				DisplayMode []modes= new DisplayMode[100];
				Format []formats = new Format[20];
				int dwNumFormats = 0;
				int dwNumModes = 0;
				int dwNumAdapterModes = Manager.Adapters[iAdapter].SupportedDisplayModes.Count;

				// Add the adapter's current desktop format to the list of formats
				formats[dwNumFormats++] = pAdapter.d3ddmDesktop.Format;

				IEnumerator ie = Manager.Adapters[iAdapter].SupportedDisplayModes.GetEnumerator();
				while(ie.MoveNext()) {
					// Get the display mode attributes
					object ob = ie.Current;
					DisplayMode _dm=(DisplayMode)ob;
					// Filter out low-resolution modes
					if( _dm.Width  < 640 || _dm.Height < 400 )
						continue;

					// Check if the mode already exists (to filter out refresh rates)
					int m;
					for(m=0; m<dwNumModes; m++) {
						if( ( modes[m].Width  == _dm.Width  ) &&
							( modes[m].Height == _dm.Height ) &&
							( modes[m].Format == _dm.Format ) )
							break;
					}

					// If we found a new mode, add it to the list of modes
					if( m == dwNumModes ) {
						modes[dwNumModes].Width       = _dm.Width;
						modes[dwNumModes].Height      = _dm.Height;
						modes[dwNumModes].Format      = _dm.Format;
						modes[dwNumModes].RefreshRate = 0;
						dwNumModes++;
						// Check if the mode's format already exists
						int f;
						for( f=0; f<dwNumFormats; f++ ) {
							if( _dm.Format == formats[f] )
								break;
						}
						// If the format is new, add it to the list
						if( f == dwNumFormats )
							formats[dwNumFormats++] = _dm.Format;
					}
				}

				// Sort the list of display modes (by format, then width, then height)
				Array.Sort(modes,new IDispModesComparer());

				// Add devices to adapter
				for( int iDevice = 0; iDevice < dwNumDeviceTypes; iDevice++ ) {
					// Fill in device info
					D3DDeviceInfo pDevice;
					pDevice                 = pAdapter.devices[pAdapter.dwNumDevices];
					pDevice.DeviceType     = DeviceTypes[iDevice];
					try {
						pDevice.d3dCaps = Manager.GetDeviceCaps(iAdapter, DeviceTypes[iDevice]).DeviceCaps;
					}
					catch {
						//ReportError(e);
						//String2Err(e.Message);
						continue;
					}
					pDevice.strDesc        = strDeviceDescs[iDevice];
					pDevice.dwNumModes     = 0;
					pDevice.dwCurrentMode  = 0;
					pDevice.bCanDoWindowed = false;
					pDevice.bWindowed      = false;
					pDevice.MultiSampleTypeFullscreen = MultiSampleType.None;
					pDevice.MultiSampleTypeWindowed = MultiSampleType.None;

					// Examine each format supported by the adapter to see if it will
					// work with this device and meets the needs of the application.
					bool   []bFormatConfirmed=new bool[20];
					CreateFlags []dwBehavior=new CreateFlags[20];
					DepthFormat []fmtDepthStencil=new DepthFormat[20];

					for( int f=0; f<dwNumFormats; f++ ) {
						bFormatConfirmed[f] = false;
						fmtDepthStencil[f] = (DepthFormat)Format.Unknown;

						// Skip formats that cannot be used as render targets on this device
						if( !Manager.CheckDeviceType( iAdapter, pDevice.DeviceType, formats[f], formats[f],false)) 
							continue;

						if( pDevice.DeviceType == DeviceType.Hardware ) {
							// This system has a HAL device
							bHALExists = true;

							if(true /*(pDevice.d3dCaps.Caps2 & (int)CONST_D3DCAPS2FLAGS.D3DCAPS2_CANRENDERWINDOWED) != 0*/) {
								// HAL can run in a window for some mode
								bHALIsWindowedCompatible = true;

								if( f == 0 ) {
									// HAL can run in a window for the current desktop mode
									bHALIsDesktopCompatible = true;
								}
							}
						}

						// Confirm the device/format for HW vertex processing
						if( pDevice.d3dCaps.SupportsHardwareTransformAndLight ) {
							if( pDevice.d3dCaps.SupportsPureDevice) {
								dwBehavior[f] = CreateFlags.HardwareVertexProcessing | CreateFlags.PureDevice;

								if( SUCCEEDED(ConfirmDevice( pDevice.d3dCaps, dwBehavior[f], formats[f])) )
									bFormatConfirmed[f] = true;
							}

							if ( false == bFormatConfirmed[f] ) {
								dwBehavior[f] = CreateFlags.HardwareVertexProcessing;

								if( SUCCEEDED(ConfirmDevice( pDevice.d3dCaps, dwBehavior[f], formats[f])) )
									bFormatConfirmed[f] = true;
							}

							if ( false == bFormatConfirmed[f] ) {
								dwBehavior[f] = CreateFlags.MixedVertexProcessing;

								if( SUCCEEDED(ConfirmDevice( pDevice.d3dCaps, dwBehavior[f], formats[f])) )
									bFormatConfirmed[f] = true;
							}
						}

						// Confirm the device/format for SW vertex processing
						if( false == bFormatConfirmed[f] ) {
							dwBehavior[f] = CreateFlags.SoftwareVertexProcessing;

							if( SUCCEEDED(ConfirmDevice( pDevice.d3dCaps, dwBehavior[f], formats[f])) )
								bFormatConfirmed[f] = true;
						}

						// Find a suitable depth/stencil buffer format for this device/format
						if( bFormatConfirmed[f] && m_bUseDepthBuffer ) {
							if( !FindDepthStencilFormat( iAdapter, pDevice.DeviceType,
								formats[f], ref fmtDepthStencil[f] ) ) {
								bFormatConfirmed[f] = false;
							}
						}
					}

					// Add all enumerated display modes with confirmed formats to the
					// device's list of valid modes
					int m;
					for( m=0; m<dwNumModes; m++ ) {
						for( int f=0; f<dwNumFormats; f++ ) {
							if( modes[m].Format == formats[f] ) {
								if( bFormatConfirmed[f] == true ) {
									// Add this mode to the device's list of valid modes
									pDevice.modes[pDevice.dwNumModes].Width      = modes[m].Width;
									pDevice.modes[pDevice.dwNumModes].Height     = modes[m].Height;
									pDevice.modes[pDevice.dwNumModes].Format     = modes[m].Format;
									pDevice.modes[pDevice.dwNumModes].dwBehavior = dwBehavior[f];
									pDevice.modes[pDevice.dwNumModes].DepthStencilFormat = fmtDepthStencil[f];
									pDevice.dwNumModes++;

									if( pDevice.DeviceType == DeviceType.Hardware)
										bHALIsSampleCompatible = true;
								}
							}
						}
					}// Loop through all dwNumModes end

					// Select any 640x480 mode for default (but prefer a 16-bit mode)
					for( m=0; m<pDevice.dwNumModes; m++ ) {
						if( pDevice.modes[m].Width==640 && pDevice.modes[m].Height==480 ) {
							pDevice.dwCurrentMode = m;
							if( pDevice.modes[m].Format == Format.R5G6B5 ||
								pDevice.modes[m].Format == Format.X1R5G5B5 ||
								pDevice.modes[m].Format == Format.A1R5G5B5 ) {
								break;
							}
						}
					}

					// Check if the device is compatible with the desktop display mode
					// (which was added initially as formats[0])
					if( bFormatConfirmed[0]) {
						pDevice.bCanDoWindowed = true;
						pDevice.bWindowed      = true;
					}

					// If valid modes were found, keep this device
					if( pDevice.dwNumModes > 0 )
						pAdapter.dwNumDevices++;
				}// Loop through dwNumDeviceTypes(HAL and REF) end

				// If valid devices were found, keep this adapter
				if( pAdapter.dwNumDevices > 0 )
					m_dwNumAdapters++;
			}//Loop through all the adapters end

			// Return an error if no compatible devices were found
			if( 0 == m_dwNumAdapters ) {
				DisplayErrorMsg(DxError.D3DAPPERR_NOCOMPATIBLEDEVICES);
				return false;
			}

			// Pick a default device that can render into a window
			// (This code assumes that the HAL device comes before the REF
			// device in the device array).
			for( int a=0; a<m_dwNumAdapters; a++ ) {
				for( int d=0; d < m_Adapters[a].dwNumDevices; d++ ) {
					if( m_Adapters[a].devices[d].bWindowed ) {
						m_Adapters[a].dwCurrentDevice = d;
						m_dwAdapter = a;
						m_bWindowed = true;

						// Display a warning message
						if( m_Adapters[a].devices[d].DeviceType == DeviceType.Reference ) {
							if( !bHALExists )
								DisplayErrorMsg(DxError.D3DAPPERR_NOHARDWAREDEVICE, true);
							else if( !bHALIsSampleCompatible )
								DisplayErrorMsg(DxError.D3DAPPERR_HALNOTCOMPATIBLE, true);
							else if( !bHALIsWindowedCompatible )
								DisplayErrorMsg(DxError.D3DAPPERR_NOWINDOWEDHAL, true);
							else if( !bHALIsDesktopCompatible )
								DisplayErrorMsg(DxError.D3DAPPERR_NODESKTOPHAL, true);
							else // HAL is desktop compatible, but not sample compatible
								DisplayErrorMsg(DxError.D3DAPPERR_NOHALTHISMODE, true);
						}
						return true;
					}
				}
			}
			DisplayErrorMsg( DxError.D3DAPPERR_NOWINDOWABLEDEVICES);
			return false;
		}

		bool	m_qSceneChanged;
		public bool	SceneChanged {
			get {
				return m_qSceneChanged;
			}set {
				 m_qSceneChanged=value;
			 }
		}

		static public void DisplayErrorMsg(DxError e){
			DisplayErrorMsg(e,false);			
		}
		//
		//		static public bool DisplayD3DErrMsg(CONST_D3DERR e){
		//			string sMsg;
		//			switch( e ){
		//			case CONST_D3DERR.D3DERR_CONFLICTINGRENDERSTATE :
		//				sMsg="The currently set render states cannot be used together."; break;
		//			case CONST_D3DERR.D3DERR_CONFLICTINGTEXTUREFILTER :
		//				sMsg="The current texture filters cannot be used together."; break;
		//			case CONST_D3DERR.D3DERR_CONFLICTINGTEXTUREPALETTE :
		//				sMsg="The current textures cannot be used simultaneously. This generally occurs when a multitexture device requires that all palletized textures simultaneously enabled also share the same palette."; break;
		//			case CONST_D3DERR.D3DERR_DEVICELOST :
		//				sMsg="The device is lost and cannot be restored at the current time, so rendering is not possible."; break;
		//			case CONST_D3DERR.D3DERR_DEVICENOTRESET :
		//				sMsg="The device cannot be reset."; break;
		//			case CONST_D3DERR.D3DERR_DRIVERINTERNALERROR :
		//				sMsg="Internal driver error."; break;
		//			case CONST_D3DERR.D3DERR_INVALIDCALL :
		//				sMsg="The method call is invalid. For example, a method's parameter may have an invalid value."; break;
		//			case CONST_D3DERR.D3DERR_INVALIDDEVICE :
		//				sMsg="The requested device type is not valid."; break;
		//			case CONST_D3DERR.D3DERR_MOREDATA :
		//				sMsg="There is more data available than the specified buffer size can hold."; break;
		//			case CONST_D3DERR.D3DERR_NOTAVAILABLE :
		//				sMsg="This device does not support the queried technique."; break;
		//			case CONST_D3DERR.D3DERR_NOTFOUND :
		//				sMsg="The requested item was not found."; break;
		//			case CONST_D3DERR.D3DERR_OUTOFVIDEOMEMORY :
		//				sMsg="Direct3D does not have enough display memory to perform the operation."; break;
		//			case CONST_D3DERR.D3DERR_TOOMANYOPERATIONS :
		//				sMsg="The application is requesting more texture-filtering operations than the device supports."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDALPHAARG :
		//				sMsg="The device does not support a specified texture-blending argument for the alpha channel."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDALPHAOPERATION :
		//				sMsg="The device does not support a specified texture-blending operation for the alpha channel."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDCOLORARG :
		//				sMsg="The device does not support a specified texture-blending argument for color values."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDCOLOROPERATION :
		//				sMsg="The device does not support a specified texture-blending operation for color values."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDFACTORVALUE :
		//				sMsg="The device does not support the specified texture factor value."; break;
		//			case CONST_D3DERR.D3DERR_UNSUPPORTEDTEXTUREFILTER :
		//				sMsg="The device does not support the specified texture filter."; break;
		//			case CONST_D3DERR.D3DERR_WRONGTEXTUREFORMAT :
		//				sMsg="The pixel format of the texture surface is not valid."; break;
		//			default:
		//				return false;
		//			}
		//			MessageBox.Show(sMsg,"Direct3D error");
		//			return true;
		//		}
		static public void DisplayErrorMsg(DxError e, bool swtoref){
			string sMsg;
			switch( e ){
				case DxError.D3DAPPERR_NODIRECT3D:
					sMsg = "Could not initialize Direct3D. You may\n" +
						"want to check that the latest version of\n" +
						"DirectX is correctly installed on your\n" +
						"system.";
					break;
				case DxError.D3DAPPERR_NOCOMPATIBLEDEVICES:
					sMsg ="Could not find any compatible Direct3D\ndevices.";
					break;
				case DxError.D3DAPPERR_NOWINDOWABLEDEVICES:
					sMsg ="This sample cannot run in a desktop\n" +
						"window with the current display settings.\n" +
						"Please change your desktop settings to a\n" +
						"16- or 32-bit display mode and re-run";
					break;
				case DxError.D3DAPPERR_NOHARDWAREDEVICE:
					sMsg ="No hardware-accelerated Direct3D devices\nwere found.";
					break;
				case DxError.D3DAPPERR_HALNOTCOMPATIBLE:
					sMsg ="This application requires functionality that is\n" +
						"not available on your Direct3D hardware\naccelerator.";
					break;
				case DxError.D3DAPPERR_NOWINDOWEDHAL:
					sMsg ="Your Direct3D hardware accelerator cannot\n" +
						"render into a window.\n";
					break;
				case DxError.D3DAPPERR_NODESKTOPHAL:
					sMsg ="Your Direct3D hardware accelerator cannot\n" +
						"render into a window with the current\n" +
						"desktop display settings.";
					break;
				case DxError.D3DAPPERR_NOHALTHISMODE:
					sMsg ="This application requires functionality that is\n" +
						"not available on your Direct3D hardware\n" +
						"accelerator with the current desktop display\n" +
						"settings.";
					break;
				case DxError.D3DAPPERR_MEDIANOTFOUND:
					sMsg ="Could not load required media.";
					break;
				case DxError.D3DAPPERR_RESIZEFAILED:
					sMsg ="Could not reset the Direct3D device.";
					break;
				case DxError.D3DAPPERR_NONZEROREFCOUNT:
					sMsg ="A D3D object has a non-zero reference\n" +
						"count (meaning things were not properly\n" +
						"cleaned up).";
					break;
				default:
					sMsg ="Generic application error";
					break;
			}
			if(swtoref){
				sMsg += "\n\nSwitching to the reference rasterizer,\n" +
					"a software device that implements the entire\n" +
					"Direct3D feature set, but runs very slowly.";
			}
			MessageBox.Show(sMsg,"DirectX error");
		}

		bool FindDepthStencilFormat(int iAdapter,
			DeviceType DeviceType,
			Format TargetFormat,
			ref DepthFormat pDepthStencilFormat) {
			//const int D3DUSAGE_RENDERTARGET = 1;
			Usage D3DUSAGE_DEPTHSTENCIL = Usage.DepthStencil;
			if( m_dwMinDepthBits <= 16 && m_dwMinStencilBits == 0 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, Usage.DepthStencil,
					ResourceType.Surface,
					(Format)DepthFormat.D16 )) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D16)) {
						pDepthStencilFormat = DepthFormat.D16;
						return true;
					}
				}
			}

			if( m_dwMinDepthBits <= 15 && m_dwMinStencilBits <= 1 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, Usage.DepthStencil, ResourceType.Surface,
					DepthFormat.D15S1)) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D15S1 )) {
						pDepthStencilFormat = DepthFormat.D15S1;
						return true;
					}
				}
			}

			if( m_dwMinDepthBits <= 24 && m_dwMinStencilBits == 0 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, Usage.DepthStencil, ResourceType.Surface,
					DepthFormat.D24S8 )) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D24S8) ) {
						pDepthStencilFormat = DepthFormat.D24S8;
						return true;
					}
				}
			}

			if( m_dwMinDepthBits <= 24 && m_dwMinStencilBits <= 4 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, D3DUSAGE_DEPTHSTENCIL, ResourceType.Surface,
					DepthFormat.D24X4S4 )) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D24X4S4 )) {
						pDepthStencilFormat = DepthFormat.D24X4S4;
						return true;
					}
				}
			}

			if( m_dwMinDepthBits <= 24 && m_dwMinStencilBits <= 8 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, D3DUSAGE_DEPTHSTENCIL, ResourceType.Surface,
					DepthFormat.D24S8 )) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D24S8 ) ) {
						pDepthStencilFormat = DepthFormat.D24S8;
						return true;
					}
				}
			}

			if( m_dwMinDepthBits <= 32 && m_dwMinStencilBits == 0 ) {
				if( Manager.CheckDeviceFormat( iAdapter, DeviceType,
					TargetFormat, D3DUSAGE_DEPTHSTENCIL, ResourceType.Surface,
					DepthFormat.D32 )) {
					if( Manager.CheckDepthStencilMatch( iAdapter, DeviceType,
						TargetFormat, TargetFormat, DepthFormat.D32 ) ) {
						pDepthStencilFormat = DepthFormat.D32;
						return true;
					}
				}
			}

			return false;
		}
		int Initialize3DEnvironment() {
			int hr=S_OK;

			D3DAdapterInfo pAdapterInfo = m_Adapters[m_dwAdapter];
			D3DDeviceInfo  pDeviceInfo  = pAdapterInfo.devices[pAdapterInfo.dwCurrentDevice];
			D3DModeInfo    pModeInfo    = pDeviceInfo.modes[pDeviceInfo.dwCurrentMode];

			// Prepare window for possible windowed/fullscreen change
			AdjustWindowForChange();

			// Set up the presentation parameters
			m_d3dpp = new PresentParameters();
			m_d3dpp.Windowed               = pDeviceInfo.bWindowed ? true : false;
			m_d3dpp.BackBufferCount        = 1;
			if( pDeviceInfo.bWindowed )
				m_d3dpp.MultiSample   = pDeviceInfo.MultiSampleTypeWindowed;
			else
				m_d3dpp.MultiSample    = pDeviceInfo.MultiSampleTypeFullscreen;
			m_d3dpp.SwapEffect             = SwapEffect.Discard;
			m_d3dpp.EnableAutoDepthStencil = m_bUseDepthBuffer ? true : false;
			m_d3dpp.AutoDepthStencilFormat = pModeInfo.DepthStencilFormat;
			m_d3dpp.DeviceWindow          = m_hWnd;
			if( m_bWindowed ) {
				Rectangle rc = m_hWnd.ClientRectangle;
				m_d3dpp.BackBufferWidth  = rc.Right - rc.Left;
				m_d3dpp.BackBufferHeight = rc.Bottom - rc.Top;
				m_d3dpp.BackBufferFormat = pAdapterInfo.d3ddmDesktop.Format;
				//m_d3dpp.BackBufferFormat = CONST_D3DFORMAT.D3DFMT_A8R8G8B8;
			}
			else {
				m_d3dpp.BackBufferWidth  = pModeInfo.Width;
				m_d3dpp.BackBufferHeight = pModeInfo.Height;
				m_d3dpp.BackBufferFormat = pModeInfo.Format;
			}

			// Create the device
			CreateFlags crf = pModeInfo.dwBehavior;
			m_pd3dDevice = new Device(
				m_dwAdapter,
				pDeviceInfo.DeviceType,
				m_hWnd,
				crf | CreateFlags.FpuPreserve | CreateFlags.MultiThreaded,
				m_d3dpp);
			if( m_pd3dDevice != null ) {
				// When moving from fullscreen to windowed mode, it is important to
				// adjust the window size after recreating the device rather than
				// beforehand to ensure that you get the window size you want.  For
				// example, when switching from 640x480 fullscreen to windowed with
				// a 1000x600 window on a 1024x768 desktop, it is impossible to set
				// the window size to 1000x600 until after the display mode has
				// changed to 1024x768, because windows cannot be larger than the
				// desktop.
				if( m_bWindowed ) {
					// TODO:
#if false
					SetWindowPos( m_hWnd, HWND_NOTOPMOST,
						m_rcWindowBounds.left, m_rcWindowBounds.top,
						( m_rcWindowBounds.right - m_rcWindowBounds.left ),
						( m_rcWindowBounds.bottom - m_rcWindowBounds.top ),
						SWP_SHOWWINDOW );
#endif
				}

				// Store device Caps
				m_d3dCaps = m_pd3dDevice.DeviceCaps;
				m_dwCreateFlags = pModeInfo.dwBehavior;

				// Store device description
				if( pDeviceInfo.DeviceType == DeviceType.Reference )
					m_strDeviceStats="REF";
				else if( pDeviceInfo.DeviceType == DeviceType.Hardware )
					m_strDeviceStats="HAL";
				else if( pDeviceInfo.DeviceType == DeviceType.Software )
					m_strDeviceStats="SW";

				if( (((int)(pModeInfo.dwBehavior) & (int)CreateFlags.HardwareVertexProcessing)!= 0) &&
					(((int)(pModeInfo.dwBehavior) & (int)CreateFlags.PureDevice)!=0) ) {
					if( pDeviceInfo.DeviceType == DeviceType.Hardware )
						m_strDeviceStats+=" (pure hw vp)";
					else
						m_strDeviceStats+=" (simulated pure hw vp)";
				}
				else if( ((int)(pModeInfo.dwBehavior) & (int)CreateFlags.HardwareVertexProcessing)!=0) {
					if( pDeviceInfo.DeviceType == DeviceType.Hardware )
						m_strDeviceStats+=" (hw vp)";
					else
						m_strDeviceStats+=" (simulated hw vp)";
				}
				else if( ((int)(pModeInfo.dwBehavior) & (int)CreateFlags.MixedVertexProcessing)!=0 ) {
					if( pDeviceInfo.DeviceType == DeviceType.Hardware )
						m_strDeviceStats+=" (mixed vp)";
					else
						m_strDeviceStats+=" (simulated mixed vp)";
				}
				else if( ((int)(pModeInfo.dwBehavior) & (int)CreateFlags.SoftwareVertexProcessing)!=0 ) {
					m_strDeviceStats+=" (sw vp)";
				}

				if( pDeviceInfo.DeviceType == DeviceType.Hardware ) {
					m_strDeviceStats+=": ";
					m_strDeviceStats+= pAdapterInfo.d3dAdapterIdentifier.ToString();
				}


				// Set up the fullscreen cursor
				//TODO: fullscreen
#if false
				if( m_bShowCursorWhenFullscreen && !m_bWindowed ) {
					HCURSOR hCursor;
					hCursor = (HCURSOR)GetClassLong( m_hWnd, GCL_HCURSOR );
					D3DUtil_SetDeviceCursor( m_pd3dDevice, hCursor, TRUE );
					m_pd3dDevice.ShowCursor( TRUE );
				}

				// Confine cursor to fullscreen window
				if( m_bClipCursorWhenFullscreen ) {
					if (!m_bWindowed ) {
						RECT rcWindow;
						GetWindowRect( m_hWnd, &rcWindow );
						ClipCursor( &rcWindow );
					}
					else {
						ClipCursor( NULL );
					}
				}
#endif
				m_pd3dDevice.DeviceResizing += new System.ComponentModel.CancelEventHandler(this.EnvironmentResized);


				// Initialize the app's device-dependent objects
				hr = InitDeviceObjects();
				if( SUCCEEDED( hr ) ) {
					hr = RestoreDeviceObjects();
					if( SUCCEEDED(hr) ) {
						return hr;
					}
				}

				// Cleanup before we try again
				InvalidateDeviceObjects();
				DeleteDeviceObjects();
				m_pd3dDevice = null;
			}

			// If that failed, fall back to the reference rasterizer
			if( pDeviceInfo.DeviceType == DeviceType.Hardware) {
				// Select the default adapter
				m_dwAdapter = 0;
				pAdapterInfo = m_Adapters[m_dwAdapter];

				// Look for a software device
				for( int i=0; i<pAdapterInfo.dwNumDevices; i++ ) {
					if( pAdapterInfo.devices[i].DeviceType == DeviceType.Reference ) {
						pAdapterInfo.dwCurrentDevice = i;
						pDeviceInfo = pAdapterInfo.devices[i];
						m_bWindowed = pDeviceInfo.bWindowed;
						break;
					}
				}

				// Try again, this time with the reference rasterizer
				if( pAdapterInfo.devices[pAdapterInfo.dwCurrentDevice].DeviceType ==
					DeviceType.Reference ) {
					//TODO:
#if false
					// Make sure main window isn't topmost, so error message is visible
					SetWindowPos( m_hWnd, HWND_NOTOPMOST,
						m_rcWindowBounds.left, m_rcWindowBounds.top,
						( m_rcWindowBounds.right - m_rcWindowBounds.left ),
						( m_rcWindowBounds.bottom - m_rcWindowBounds.top ),
						SWP_SHOWWINDOW );
#endif
					AdjustWindowForChange();

					// Let the user know we are switching from HAL to the reference rasterizer
					DisplayErrorMsg( DxError.D3DAPPERR_NOHALTHISMODE, true);

					hr = Initialize3DEnvironment();
				}
			}

			return hr;
		}
		/// <summary>
		/// Fired when our environment was resized
		/// </summary>
		/// <param name="sender">the device that's resizing our environment</param>
		/// <param name="e">Set the cancel member to true to turn off automatic device reset</param>
		public void EnvironmentResized(object sender, System.ComponentModel.CancelEventArgs e) {
			// Check to see if we're minimizing and our rendering object
			// is not the form, if so, cancel the resize
			if (m_hWnd.ParentForm.WindowState == System.Windows.Forms.FormWindowState.Minimized)
				e.Cancel = true;
		}

		//-----------------------------------------------------------------------------
		// Name: AdjustWindowForChange()
		// Desc: Prepare the window for a possible change between windowed mode and
		//       fullscreen mode.  This function is virtual and thus can be overridden
		//       to provide different behavior, such as switching to an entirely
		//       different window for fullscreen mode (as in the MFC sample apps).
		//-----------------------------------------------------------------------------
		virtual public bool AdjustWindowForChange() {
			// TODO: change window style
			if( m_bWindowed ) {
				// Set windowed-mode style
				//SetWindowLong( m_hWnd, GWL_STYLE, m_dwWindowStyle );
			}
			else {
				// Set fullscreen-mode style
				//SetWindowLong( m_hWnd, GWL_STYLE, WS_POPUP|WS_SYSMENU|WS_VISIBLE );
			}
			return true;
		}
		static public bool FAILED(int hr){
			return hr < 0;
		}
		static public bool SUCCEEDED(int hr){
			return hr >= 0;
		}
		public int Render3DEnvironment() {
			int hr;
			try {
				m_pd3dDevice.TestCooperativeLevel();
			}
			catch(Microsoft.DirectX.DirectXException e) {
				if(e is DeviceLostException)
					return S_OK;
				else if(e is DeviceNotResetException) {
					if( m_bWindowed ) {
						D3DAdapterInfo pAdapterInfo = m_Adapters[m_dwAdapter];
						pAdapterInfo.d3ddmDesktop = Manager.Adapters[pAdapterInfo.d3dAdapterIdentifier].CurrentDisplayMode;
						m_d3dpp.BackBufferFormat = pAdapterInfo.d3ddmDesktop.Format;
					}
					if( FAILED( hr = Resize3DEnvironment() ) )
						return hr;
				}
				return S_FALSE;
			}

			// Render the scene as normal
			if( FAILED( hr = Render() ) )
				return hr;

			// flip back buffer to front
			// Show the frame on the primary surface.
			m_pd3dDevice.Present();
			return S_OK;
		}
		public int Resize3DEnvironment() {
			int hr = S_OK;
			m_bReady = false;
			int w = this.m_hWnd.ClientSize.Width;
			int h = this.m_hWnd.ClientSize.Height;
			if(w == 0 || h == 0){
				m_bReady = true;
				return S_OK;
			}
			m_d3dpp.BackBufferWidth = w;
			m_d3dpp.BackBufferHeight = h;

			// Release all vidmem objects
			if( FAILED( hr = InvalidateDeviceObjects() ) ) 
				return hr;

			// Reset the device
			try{
				m_pd3dDevice.Reset( m_d3dpp );
			}catch(Exception e){
				ReportError(e);
				//String2Err(e.Message);
				return S_FALSE;
			}


			//TODO: Set up the fullscreen cursor
#if false
			if( m_bShowCursorWhenFullscreen && !m_bWindowed ) {
				HCURSOR hCursor;
				hCursor = (HCURSOR)GetClassLong( m_hWnd, GCL_HCURSOR );
				D3DUtil_SetDeviceCursor( m_pd3dDevice, hCursor );
				m_pd3dDevice.ShowCursor( true);
			}
#endif

			// Initialize the app's device-dependent objects
			hr = RestoreDeviceObjects();
			if( FAILED(hr) )
				return hr;
			m_bReady = true;
			AfterResize(w,h);
			return S_OK;
		}
		public void Cleanup3DEnvironment(){
			m_bReady  = false;
			if( m_pd3dDevice !=null) {
				InvalidateDeviceObjects();
				DeleteDeviceObjects();
			}
			m_pd3dDevice = null;
			//			m_pD3D = null;
			//			m_dx8 = null;
			FinalCleanup();
		}
		// Overridable functions for the 3D scene created by the app
		public virtual int ConfirmDevice(DeviceCaps caps, CreateFlags createflg,Format fmt){
			return S_OK;
		}
		virtual public int OneTimeSceneInit(){ return S_OK; }
		virtual public int InitDeviceObjects(){ return S_OK; }
		virtual public int RestoreDeviceObjects(){ return S_OK; }
		virtual public int FrameMove(){ return S_OK; }
		virtual public int Render(){ return S_OK; }
		virtual public int InvalidateDeviceObjects(){ return S_OK; }
		virtual public int DeleteDeviceObjects(){ return S_OK; }
		virtual public int FinalCleanup(){ return S_OK; }
		virtual public int AfterResize(int w, int h){ return S_OK; }
		// Callback function
		private bool OnSizeMove(bool enter) {
			if(!enter)
				m_bResize = true;
			return true;
		}
		//
		public static void ReportError(Exception e) {
			String2Err(e.Message);
		}

		public Device	Renderer {
			get	{	return m_pd3dDevice;	}
		}
	}
}
