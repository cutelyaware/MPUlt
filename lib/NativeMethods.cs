using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace _3dedit {
	public class NativeMethods {
		public static readonly int Nullint = (int)0;
		public const int
			R2_XORPEN = 7,
			PS_SOLID = 0,
			PS_DASH = 1,
			FLOODFILLBORDER =  0,
			FLOODFILLSURFACE = 1,
			BITMAPINFO_MAX_COLORSIZE = 256,
			DIB_RGB_COLORS = 0,
			DIB_PAL_COLORS = 1,
			OBJ_PEN = 1,
			OBJ_BRUSH = 2,
			OBJ_DC = 3,
			OBJ_METADC = 4,
			OBJ_PAL = 5,
			OBJ_FONT = 6,
			OBJ_BITMAP = 7,
			OBJ_REGION = 8,
			OBJ_METAFILE = 9,
			OBJ_MEMDC = 10,
			OBJ_EXTPEN = 11,
			OBJ_ENHMETADC = 12,
			OBJ_ENHMETAFILE = 13,
			BI_RGB = 0,
			BI_RLE8 = 1,
			BI_RLE4 = 2,
			BI_BITFIELDS = 3,
			PLANES = 14,
			BITSPIXEL = 12,
			SRCCOPY = 0x00CC0020,
			HORZRES = 8,     /* Horizontal width in pixels               */
			VERTRES = 10;    /* Vertical height in pixels                */

		public struct RGBQUAD {
			public byte rgbBlue;
			public byte rgbGreen;
			public byte rgbRed;
			public byte rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
			public class BITMAPINFOHEADER {
			public int      biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
			public int      biWidth=0;
			public int      biHeight=0;
			public short    biPlanes=0;
			public short    biBitCount=0;
			public int      biCompression=0;
			public int      biSizeImage=0;
			public int      biXPelsPerMeter=0;
			public int      biYPelsPerMeter=0;
			public int      biClrUsed=0;
			public int      biClrImportant=0;
		}

		[StructLayout(LayoutKind.Sequential)]
			public class BITMAPINFO {
			public BITMAPINFOHEADER bmiHeader = new BITMAPINFOHEADER();

			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=BITMAPINFO_MAX_COLORSIZE*4)]
			public byte[] bmiColors=null; // RGBQUAD structs... Blue-Green-Red-Reserved, repeat...
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct PALETTEENTRY {
			public byte peRed;
			public byte peGreen;
			public byte peBlue;
			public byte peFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
			public struct BITMAPINFO_FLAT {
			public int      bmiHeader_biSize;// = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
			public int      bmiHeader_biWidth;
			public int      bmiHeader_biHeight;
			public short    bmiHeader_biPlanes;
			public short    bmiHeader_biBitCount;
			public int      bmiHeader_biCompression;
			public int      bmiHeader_biSizeImage;
			public int      bmiHeader_biXPelsPerMeter;
			public int      bmiHeader_biYPelsPerMeter;
			public int      bmiHeader_biClrUsed;
			public int      bmiHeader_biClrImportant;

			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=BITMAPINFO_MAX_COLORSIZE*4)]
			public byte[] bmiColors; // RGBQUAD structs... Blue-Green-Red-Reserved, repeat...
		}

		[DllImport("gdi32")]
		public static extern int GetPaletteEntries(int hpal, int iStartIndex, int nEntries, byte[] lppe);
		[DllImport("gdi32")]
		public static extern int GetSystemPaletteEntries(int hdc, int iStartIndex, int nEntries, byte[] lppe);
		[DllImport("gdi32")]
		public static extern int CreateDIBSection(int hdc, ref NativeMethods.BITMAPINFO_FLAT bmi, int iUsage, ref int ppvBits, int hSection, int dwOffset);
		[DllImport("gdi32")]
		public static extern int GetObjectType(int hobject);
		[DllImport("gdi32")]
		public static extern int CreateCompatibleDC(int hDC);
		[DllImport("gdi32")]
		public static extern int CreateCompatibleBitmap(int hDC, int width, int height);
		[DllImport("gdi32")]
		public static extern int GetDIBits(int hdc, int hbm, int arg1, int arg2, int arg3, NativeMethods.BITMAPINFOHEADER bmi, int arg5);
		[DllImport("gdi32")]
		public static extern int GetDIBits(int hdc, int hbm, int arg1, int arg2, int arg3, ref NativeMethods.BITMAPINFO_FLAT bmi, int arg5);
		[DllImport("gdi32")]
		public static extern int SelectObject(int hdc, int obj);
		[DllImport("gdi32")]
		public static extern bool DeleteObject(int hObject);
		[DllImport("gdi32")]
		public static extern bool DeleteDC(int hDC);
		[DllImport("gdi32")]
		public static extern bool BitBlt(int hDC, int x, int y, int nWidth, int nHeight,
			int hSrcDC, int xSrc, int ySrc, int dwRop);
		[DllImport("gdi32")]
		public static extern int GetDeviceCaps(int hDC, int nIndex);
		[DllImport("gdi32")]
		public static extern bool ExtFloodFill(
			int hdc,          // handle to DC
			int nXStart,      // starting x-coordinate 
			int nYStart,      // starting y-coordinate 
			int crColor,	  // fill color
			int fuFillType    // fill type
			);
		[DllImport("gdi32")]
		public static extern int SetROP2(
			int hDC,          // handle to DC
			int drawMode      // starting x-coordinate 
			);
		[DllImport("gdi32")]
		public static extern int CreatePen(
			int fnPenStyle,    // pen style
			int nWidth,        // pen width
			int crColor   // pen color
			);
		[DllImport("gdi32")]
		public static extern bool MoveToEx(
			int hdc,          // handle to device context
			int X,            // x-coordinate of new current position
			int Y,            // y-coordinate of new current position
			int lpPoint   // old current position
			);
		[DllImport("gdi32")]
		public static extern bool LineTo(
			int hdc,    // device context handle
			int nXEnd,  // x-coordinate of ending point
			int nYEnd   // y-coordinate of ending point
			);
		[DllImport("Kernel32")]
		unsafe public static extern void *LocalFree(
			void *ptr);
		[DllImport("Kernel32")]
		unsafe public static extern int FreeLibrary(
			void *ptr);
		[DllImport("Kernel32")]
		unsafe public static extern void* LoadLibraryEx(
			string sName,
			void *hFile,
			int iFlags
			);
		[DllImport("Kernel32")]
		unsafe public static extern int FormatMessageA(
			int dwFlags,        // source and processing options
			void *lpSource,   // message source
			int dwMessageId,  // message identifier
			int dwLanguageId, // language identifier
			byte **lpBuffer,     // message buffer
			int nSize,        // maximum size of message buffer
			byte *Arguments  // array of message inserts
			);
		[DllImport("User32")]
		public static extern void PostQuitMessage(int resultVal);

		public static readonly uint MB_ICONHAND = (uint) 0x00000010L;
		public static readonly uint MB_ICONQUESTION = (uint) 0x00000020L;
		public static readonly uint MB_ICONEXCLAMATION = (uint) 0x00000030L;
		public static readonly uint MB_ICONASTERISK = (uint) 0x00000040L;
		[DllImport("User32")]
		public static extern void MessageBeep(uint mtype);
		[DllImport("Winmm")]
		public static extern int timeGetTime();		
		public static readonly int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
		public static readonly int FORMAT_MESSAGE_IGNORE_INSERTS  = 0x00000200;
		public static readonly int FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
		public static readonly int FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
		public static readonly int LOAD_LIBRARY_AS_DATAFILE       = 0x00000002;
		public static readonly ushort LANG_NEUTRAL                   = 0x00; // language neutral
		public static readonly ushort SUBLANG_DEFAULT                = 0x01; // user default

		public static int MAKELANGID(int p, int s){
			return (int)((((ushort)(s)) << 10) | (ushort  )(p));
		}
		unsafe public static string HResult2ErrorText(int dwLastError) {
			return HResult2ErrorText(dwLastError, null);
		}
		//Examples:
		//NativeMethods.HResult2ErrorText(2453,"netmsg.dll");
		//result: "Could not find domain controller for this domain."
		//NativeMethods.HResult2ErrorText(5);
		//result: "Access is denied."
		unsafe public static string HResult2ErrorText(int dwLastError, string smodule) {
			string ret = "";
			int hModule = 0; // default to system source
			int dwBufferLength;
			byte *MessageBuffer;

			int dwFormatFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER |
				FORMAT_MESSAGE_IGNORE_INSERTS |
				FORMAT_MESSAGE_FROM_SYSTEM ;

			if(smodule!=null) {
				hModule = (int)LoadLibraryEx(
					smodule,
					(void*)0,
					LOAD_LIBRARY_AS_DATAFILE
					);

				if(hModule != 0)
					dwFormatFlags |= FORMAT_MESSAGE_FROM_HMODULE;
				else
					return ret;
			}
			//
			// Call FormatMessage() to allow for message 
			//  text to be acquired from the system 
			//  or from the supplied module handle.
			//
			try{
				if((dwBufferLength = FormatMessageA(
					dwFormatFlags,
					(void*)hModule, // module to get message from (NULL == system)
					dwLastError,
					MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // default language
					&MessageBuffer,
					0,
					(byte*)0
					)) != 0) {
					for(int i = 0; i < dwBufferLength; i++){
						ret += (char)MessageBuffer[i];
					}
				}
				// Free the buffer allocated by the system.
				LocalFree(MessageBuffer);
			}catch(Exception e){
				ret = e.Message;
			}
			//
			// If we loaded a message source, unload it.
			//
			if(hModule != 0)
				FreeLibrary((void *)hModule);
			return ret;
		}
		public static Bitmap Graphics2Bitmap(Graphics gr, Rectangle rec) {
			int     hSrcDC, hMemDC;         // src DC and memory DC
			int     hBitmap, hOldBitmap;    // handles to deice-dependent bitmaps
			int     nX, nY, nX2, nY2;       // coordinates of rectangle to grab
			int     nWidth, nHeight;        // DIB width and height
			int     xScrn, yScrn;           // screen resolution

			hSrcDC=gr.GetHdc().ToInt32();

			// check for an empty rectangle

			if (rec.IsEmpty)
				return null;

			// create a DC for the screen and create
			// a memory DC compatible to src DC
    
			hMemDC = NativeMethods.CreateCompatibleDC(hSrcDC);

			// get points of rectangle to grab

			nX = rec.Left;
			nY = rec.Top;
			nX2 = rec.Right;
			nY2 = rec.Bottom;

			// get src resolution

			xScrn = NativeMethods.GetDeviceCaps(hSrcDC, NativeMethods.HORZRES);
			yScrn = NativeMethods.GetDeviceCaps(hSrcDC, NativeMethods.VERTRES);

			//make sure bitmap rectangle is visible

			if (nX < 0)
				nX = 0;
			if (nY < 0)
				nY = 0;
			if (nX2 > xScrn)
				nX2 = xScrn;
			if (nY2 > yScrn)
				nY2 = yScrn;

			nWidth = nX2 - nX;
			nHeight = nY2 - nY;

			// create a bitmap compatible with the src DC
			hBitmap = NativeMethods.CreateCompatibleBitmap(hSrcDC, nWidth, nHeight);

			// select new bitmap into memory DC
			hOldBitmap = NativeMethods.SelectObject(hMemDC, hBitmap);

			// bitblt src DC to memory DC
			NativeMethods.BitBlt(hMemDC, 0, 0, nWidth, nHeight, hSrcDC, nX, nY, NativeMethods.SRCCOPY);

			// select old bitmap back into memory DC and get handle to
			// bitmap of the src
    
			hBitmap = NativeMethods.SelectObject(hMemDC, hOldBitmap);

			// clean up

			NativeMethods.DeleteDC((int)hMemDC);

			gr.ReleaseHdc((IntPtr)hSrcDC);
			// return handle to the bitmap

			return Bitmap.FromHbitmap((IntPtr)hBitmap);
		}
	}
}
