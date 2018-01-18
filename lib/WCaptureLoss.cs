using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3dedit {
	enum WndMsg{
		WM_CAPTURECHANGED = 0x0215,
		WM_ERASEBKGND = 0x0014,
		WM_ENTERSIZEMOVE = 0x0231,
		WM_EXITSIZEMOVE = 0x0232,
		WM_HSCROLL = 0x0114,
		WM_VSCROLL = 0x0115
	}
	public delegate void CaptureLossHandler();
	public delegate bool EraseBackGroundHandler();
	public delegate bool SizeMoveHandler(bool enter);
	public class WCaptureLoss: NativeWindow {
		public event CaptureLossHandler OnCaptureLoss;
		public event EraseBackGroundHandler OnEraseBackGround;
		public event SizeMoveHandler OnSizeMove;
		public IntPtr HorHwnd = IntPtr.Zero;
		public IntPtr VerHwnd = IntPtr.Zero;
		protected override void WndProc(ref Message msg) {			
			switch((WndMsg)msg.Msg){
				case WndMsg.WM_CAPTURECHANGED: {
					if(OnCaptureLoss != null){
						OnCaptureLoss();
					}
					break;
				}
				case WndMsg.WM_ERASEBKGND: {
					if(OnEraseBackGround != null){
						if(OnEraseBackGround())
							return;
					}
					break;
				}
				case WndMsg.WM_EXITSIZEMOVE: {
					if(OnSizeMove != null){
						OnSizeMove(false);
					}
					break;
				}
				case WndMsg.WM_ENTERSIZEMOVE: {
					if(OnSizeMove != null){
						OnSizeMove(true);
					}
					break;
				}
				case WndMsg.WM_HSCROLL: {
					HorHwnd = msg.HWnd;
					break;
				}
				case WndMsg.WM_VSCROLL: {
					VerHwnd = msg.HWnd;
					break;
				}
				default:
					break;
			}
			base.WndProc(ref msg);
		}
	}
}
// EOF

