using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace _3dedit{
    public interface IKeysProvider {
        bool qLeftMouse { get; }
        bool qRightMouse { get; }
        bool qA { get; }
        bool qShift { get; }
        bool qCtrl { get; }
        bool qAlt { get; }
        bool qX { get; }
        bool qY { get; }
        bool qZ { get; }
        bool Eq(IKeysProvider m);
    }

	public interface IDXObject {
		void Render(S3DirectX d3dDevice);
		bool GetBounds(out RPoint min, out RPoint max);
		void CleanUp();
		bool NeedLightning	{	get;	}
	}

	public interface IDXControl {
		void SetSceneChanged();
		S3DirectX	Scene{ get; }
		ArrayList	DXObjects	{	get;	}
		FillMode FillMode{ get; set; }
	}

    public interface ICamera {
    }

}
