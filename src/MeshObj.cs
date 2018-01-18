using System;
using System.Drawing;
using System.Collections;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
    class StkMesh {
        internal PMesh Base;
        internal int NFace;
        internal double[,] ExtraTwist;  // 4D, for animation
        internal float[] Coords;
        internal Vector3 []Coords3D;
        internal int Col;
        internal double[,] Matr;
        internal double[] FCtr;
        internal bool qBelow;
        static double[] CurPt;


        internal StkMesh(PMesh _base,int nf,double[,] matr) {
            Base=_base;
            int nv=Base.NV;
            int dim=Base.PDim;
            Matr=matr;
            Coords=new float[nv*dim];
            Coords3D=new Vector3[nv];
            CurPt=new double[dim];
            NFace=nf;
        }
        internal void SetCoord(double fs,double ss) {
            double[] fctr=Base.FCtr,ctr=Base.Ctr;
            int nv=Base.NV;
            int dim=Base.PDim;

            for(int i=0;i<nv;i++) {
                for(int j=0;j<dim;j++) {
                    double s=0;
                    for(int k=0;k<dim;k++) {
                        double bc=Base.Coords[i*dim+k];
                        bc=bc*ss+ctr[k]*(1-ss);
                        bc=bc*fs+fctr[k]*(1-fs);
                        s+=bc*Matr[k,j];
                    }
                    Coords[i*dim+j]=(float)s;
                }
            }
        }
        public void RecalcCoord(S4Camera cam) {
            qBelow=true;
            int dim=Base.PDim;
            if(ExtraTwist!=null) {
                for(int j=0;j<dim;j++) {
                    double s=0;
                    for(int k=0;k<dim;k++) s+=ExtraTwist[k,j]*FCtr[k];
                    CurPt[j]=s;
                }
                if(cam.AbovePlane(CurPt)) {
                    qBelow=false;
                    return;
                }
            }
            for(int i=0;i<Base.NV;i++) {
                if(ExtraTwist!=null) {
                    for(int j=0;j<dim;j++) {
                        double s=0;
                        for(int k=0;k<dim;k++) s+=ExtraTwist[k,j]*Coords[i*dim+k];
                        CurPt[j]=s;
                    }
                } else {
                    for(int j=0;j<dim;j++) CurPt[j]=Coords[i*dim+j];
                }
                bool x=cam.Apply(CurPt,out Coords3D[i]);
                if(!x) Coords3D[i].Z=WRONGPT;
            }
        }
        internal const float WRONGPT=-1e10f;
        public int SetBuffer2D(S4Camera cam,CustomVertex.PositionNormalColored[] buf,int ptr) {
            if(!qBelow) return ptr;
            for(int i=0;i<Base.NF*3;i+=3){
                Vector3 v0=Coords3D[Base.Faces[i]];
                Vector3 v1=Coords3D[Base.Faces[i+1]];
                Vector3 v2=Coords3D[Base.Faces[i+2]];
                if(v0.Z!=WRONGPT && v1.Z!=WRONGPT && v2.Z!=WRONGPT) {
                    bool qinv=true;
#if true
                    Vector3 v1x=v1,v2x=v2;
                    v1x.Subtract(v0); v2x.Subtract(v0);
                    Vector3 n=new Vector3(v1x.Y*v2x.Z-v1x.Z*v2x.Y,v1x.Z*v2x.X-v1x.X*v2x.Z,v1x.X*v2x.Y-v1x.Y*v2x.X);
                    if(n.Z<0) { n.Scale(-1f); qinv=false; }
                    n.Z+=n.Length()/4;
                    n.Normalize();
#else
                    Vector3 n=new Vector3(0.91f,0.3f,0.3f);
#endif
                    buf[ptr++]=new CustomVertex.PositionNormalColored(v0,n,Col);
                    buf[ptr++]=new CustomVertex.PositionNormalColored(qinv ? v1 : v2,n,Col);
                    buf[ptr++]=new CustomVertex.PositionNormalColored(qinv ? v2 : v1,n,Col);                
                }
            }
            return ptr;
        }
        public int SetBuffer1D(S4Camera cam,CustomVertex.PositionNormalColored[] buf,int ptr,int col) {
            if(!qBelow) return ptr;
            for(int i=0;i<Base.NE*2;i+=2) {
                Vector3 v0=Coords3D[Base.Edges[i]];
                Vector3 v1=Coords3D[Base.Edges[i+1]];
                if(v0.Z!=WRONGPT && v1.Z!=WRONGPT) {
                    buf[ptr++]=new CustomVertex.PositionNormalColored(v0,new Vector3(0,0,1),col);
                    buf[ptr++]=new CustomVertex.PositionNormalColored(v1,new Vector3(0,0,1),col);
                }
            }
            return ptr;
        }
        internal int L2D() { return Base.NF*3; }
        internal int L1D() { return Base.NE*2; }

        internal double CheckRay(double x,double y,double zmin) {
            int fid;
            double fu,fv;
            return TstRay(x,y,zmin,out fid,out fu,out fv);
        }

        internal double[] FindPoint(double x,double y,ref double zmin) {
            int fid;
            double fu,fv;
            double z=TstRay(x,y,zmin,out fid,out fu,out fv);
            if(fid<0) return null;
            int dim=Base.PDim;
            int v0=Base.Faces[fid]*dim,v1=Base.Faces[fid+1]*dim,v2=Base.Faces[fid+2]*dim;
            double[] res=new double[dim];
            double[]hh=Base.Coords;
            for(int j=0;j<dim;j++) {
                double s=0;
                for(int k=0;k<dim;k++) {
                    s+=(hh[v0+k]*(1-fu-fv)+hh[v1+k]*fu+hh[v2+k]*fv)*Matr[k,j];
                }
                res[j]=s;
            }
            zmin=z;
            return res;
        }

        double TstRay(double x,double y,double zmin,out int fid,out double fu,out double fv) {
            fid=-1; fu=fv=0;
            bool xl=false,xr=false,yl=false,yr=false,zz=false;
            foreach(Vector3 v in Coords3D) {
                if(v.X>=x) xr=true;
                if(v.X<=x) xl=true;
                if(v.Y>=y) yr=true;
                if(v.Y>=y) yl=true;
                if(-v.Z<=zmin) zz=true;
            }
            if(!(xr && xl && yr && yl && zz)) return double.MaxValue;
            double zbest=zmin;
            for(int i=0;i<Base.NF*3;i+=3) {
                Vector3 v0=Coords3D[Base.Faces[i]];
                Vector3 v1=Coords3D[Base.Faces[i+1]];
                Vector3 v2=Coords3D[Base.Faces[i+2]];
                if(v0.Z!=WRONGPT && v1.Z!=WRONGPT && v2.Z!=WRONGPT) {
                    double S=((v1.X-v0.X)*(v2.Y-v0.Y)-(v1.Y-v0.Y)*(v2.X-v0.X));
                    double S1=((x-v0.X)*(v2.Y-v0.Y)-(y-v0.Y)*(v2.X-v0.X));
                    double S2=((v1.X-v0.X)*(y-v0.Y)-(v1.Y-v0.Y)*(x-v0.X));
                    if(S==0) continue;
                    double u=S1/S,v=S2/S;
                    if(u<0 || v<0 || u+v>1) continue;
                    double z=-(v0.Z*(1-u-v)+v1.Z*u+v2.Z*v);
                    if(z>=zbest) continue;
                    zbest=z; fid=i; fu=u; fv=v;
                }
            }
            return zbest;
        }
    }
    public class CubeObj: IDXObject {
        static internal int[] Colors=new int[0];

        internal int NF,NStk;
        internal StkMesh[] Stks;
        internal StkMesh[] StFaces;
        internal double[][] FPoles;
        internal bool[] BPln;
        internal int[] VertNetwork;
        internal int lVN;
        Puzzle Cube;
        VertexBuffer VBuf;
        internal int WhiteColor=-1;
        internal int ShowRank=0xfff;

        internal double FShr=0.5,SShr=0.8;
        //internal double SAxis=0.1;

        internal const int FIND_STICKER_ANY=0;
        internal const int FIND_STICKER_SELECTED=1;
        internal const int FIND_STICKER_CORNER=2;
        static int GenColor(int v) {
            int r=0,g=0,b=0;
            v++;
            int m=128;
            do {
                if((v&1)!=0) r|=m;
                if((v&2)!=0) g|=m;
                if((v&4)!=0) b|=m;
                m>>=1; v>>=3;
            } while(v!=0 && m!=0);
            return (r<<16)+(g<<8)+b;
        }

        internal unsafe void SetColorsArray(int[] cols) {
            if(cols==null) cols=Colors;
            int l=cols.Length;
            if(l>=NF) {
                Colors=cols;
                return;
            }
            int []c1=new int[NF];
            Buffer.BlockCopy(cols,0,c1,0,l*sizeof(int));
            for(int i=l;i<NF;i++) c1[i]=GenColor(i);
            Colors=c1;
        }

        internal unsafe static void SetColors(int[] cc) {
            if(cc.Length>=Colors.Length) Colors=cc;
            else Buffer.BlockCopy(cc,0,Colors,0,cc.Length*sizeof(int));
        }

        internal CubeObj(Puzzle cube) {
            InitPuzzle(cube);
        }

        internal void Dispose() {
            if(VBuf!=null) VBuf.Dispose();
            VBuf=null;
        }

        internal void InitPuzzle(Puzzle cube) {
            Cube=cube;
            int dim=Cube.Str.Dim;
            NF=Cube.Str.Faces.Length;
            NStk=Cube.Str.NStickers;
            SetColorsArray(null);
            StFaces=new StkMesh[NF];
            FPoles=new double[NF][];
            Stks=new StkMesh[NStk];
            BPln=new bool[NF];
            int[] vn=new int[100000*4];
            lVN=0;

            double[][][] FFPoles=new double[NF][][];
            for(int i=0;i<NF;i++) {
                PFace F=Cube.Str.Faces[i];
                StFaces[i]=new StkMesh(F.Base.FaceMesh,i,F.Matrix);
                FPoles[i]=F.Pole;
                int fstk=F.FirstSticker;
                for(int j=0;j<F.Base.NStickers;j++) {
                    Stks[fstk+j]=new StkMesh(F.Base.StickerMesh[j],i,F.Matrix);
                    Stks[fstk+j].FCtr=F.Pole;
                }
                double[][]baseFFP=F.Base.FPoles;
                FFPoles[i]=new double[baseFFP.Length][];
                for(int j=0;j<baseFFP.Length;j++) FFPoles[i][j]=PGeom.ApplyMatrix(F.Matrix,baseFFP[j]);
            }
            for(int i=0;i<NF;i++) StFaces[i].SetCoord(1,1);

            for(int i=1;i<NF;i++) {
                for(int j=0;j<i;j++) {
                    double[][]pi=FFPoles[i],pj=FFPoles[j];
                    foreach(double[]vi in pi){
                        foreach(double[] vj in pj){
                            if(PtEq(vi,vj,dim)) goto _1;
                        }
                    }
                    continue;
                _1:
                    StkMesh Fi=StFaces[i],Fj=StFaces[j];
                    float[] fpi=Fi.Coords,fpj=Fj.Coords;
                    for(int i1=0;i1<fpi.Length;i1+=dim) {
                        for(int j1=0;j1<fpj.Length;j1+=dim) {
                            if(PtEq(fpi,i1,fpj,j1,dim)) {
                                vn[lVN++]=i;
                                vn[lVN++]=i1;
                                vn[lVN++]=j;
                                vn[lVN++]=j1;
                                break;
                            }
                        }
                    }
                }
            }
            VertNetwork=new int[lVN];
            Buffer.BlockCopy(vn,0,VertNetwork,0,lVN*sizeof(int));
        }

        private bool PtEq(float[] fpi,int i1,float[] fpj,int j1,int dim) {
            for(int i=0;i<dim;i++) if(Math.Abs(fpi[i1+i]-fpj[j1+i])>0.001) return false;
            return true;
        }

        private bool PtEq(double[] vi,double[] vj,int dim) {
            for(int i=0;i<dim;i++) if(Math.Abs(vi[i]-vj[i])>0.001) return false;
            return true;
        }

        public void SetStickerColors() {
            for(int i=0;i<NStk;i++) {
                int h=Cube.Field[i]&0x3fff;
                int c=Colors[h],m=255;
                if(WhiteColor>=0) {
                    if(h==WhiteColor) c=0xffffff;
                    else { m=0x80; c=(c>>1)&0x7f7f7f; }
                }
                c|=m<<24;
                Stks[i].Col=c;
            }
        }

        public void SetStickerSize(double fs,double ss) {
            for(int i=0;i<NStk;i++) Stks[i].SetCoord(fs,ss);
            for(int i=0;i<NF;i++) StFaces[i].SetCoord(fs,1);
            //SAxis=sa;
        }
        
        public VertexBuffer GetNewBuffer(Device dev,int L) {
            VertexBuffer vbuf=null;
                try {
                    vbuf = new VertexBuffer(typeof(CustomVertex.PositionNormalColored),L,dev,
                        Usage.WriteOnly,CustomVertex.PositionNormalColored.Format,Pool.SystemMemory);
                } catch(Exception _exc) {
                    //bs.lib.Gen.LogWin.TraceError(_exc);
                    vbuf = new VertexBuffer(typeof(CustomVertex.PositionNormalColored),L,dev,
                        Usage.WriteOnly,CustomVertex.PositionNormalColored.Format,Pool.Managed);
                }

            return vbuf;
        }

        void SendVBuf(S3DirectX d3dDevice,PrimitiveType type,int nt) {
            VBuf.Unlock();
            if(nt!=0) {
                int sx=CustomVertex.PositionNormalColored.StrideSize;
                d3dDevice.Renderer.SetStreamSource(0,VBuf,0,sx);
                d3dDevice.Renderer.VertexFormat = CustomVertex.PositionNormalColored.Format;
                d3dDevice.Renderer.DrawPrimitives(type,0,nt);
            }
        }

        public void Render(S3DirectX d3dDevice) {
            int L=30000;
            d3dDevice.Renderer.SetTransform(TransformType.World,Matrix.Identity);
            d3dDevice.SetupObjectMaterial(Color.FromArgb(unchecked((int)0x10404040)));

            for(int i=0;i<NF;i++) BPln[i]=((S4Camera)(d3dDevice.Camera)).AbovePlane(FPoles[i]);

            if(VBuf==null) VBuf=GetNewBuffer(d3dDevice.Renderer,L);
            CustomVertex.PositionNormalColored[] cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
            int ptr=0;

            for(int ns=0;ns<NStk;ns++) {
                if(Stks[ns].ExtraTwist==null && BPln[Stks[ns].NFace]) continue;
                int l0=Stks[ns].L2D();
                if(ptr+l0>L) {
                    SendVBuf(d3dDevice,PrimitiveType.TriangleList,ptr/3);
                    cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
                    ptr=0;
                }
                Stks[ns].RecalcCoord((S4Camera)(d3dDevice.Camera));
                int m=1<<Math.Min(11,Stks[ns].Base.Rank);
                if((m&ShowRank)==0) continue;
                ptr=Stks[ns].SetBuffer2D((S4Camera)(d3dDevice.Camera),cbuf,ptr);
            }
            SendVBuf(d3dDevice,PrimitiveType.TriangleList,ptr/3);

            cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
            ptr=0;
            for(int ns=0;ns<NStk;ns++) { // selected stickers
                if(Stks[ns].ExtraTwist==null && BPln[Stks[ns].NFace]) continue;
                int u=Cube.Field[ns];
                int cl=(u&0x8000)!=0 ? -1 : unchecked((int)0xff202020);
                    if(ptr+Stks[ns].L1D()>L) {
                        SendVBuf(d3dDevice,PrimitiveType.LineList,ptr/2);
                        cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
                        ptr=0;
                    }
                    int m=1<<Math.Min(11,Stks[ns].Base.Rank);
                    if((m&ShowRank)==0 && Stks[ns].Base.MinBDim!=0) continue;
                    ptr=Stks[ns].SetBuffer1D((S4Camera)(d3dDevice.Camera),cbuf,ptr,cl);
                
            }
            for(int ns=0;ns<NF;ns++) {
                if(BPln[ns]) continue;
                StFaces[ns].RecalcCoord((S4Camera)(d3dDevice.Camera));
                int l0=StFaces[ns].L1D();
                if(ptr+l0>L) {
                    SendVBuf(d3dDevice,PrimitiveType.LineList,ptr/2);
                    cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
                    ptr=0;
                }
                ptr=StFaces[ns].SetBuffer1D((S4Camera)(d3dDevice.Camera),cbuf,ptr,-0x7f7f80);
            }
#if false
            int dim=Cube.Str.Dim;
            for(int i=0;i<lVN;i+=4) {
                if(ptr==L) {
                    SendVBuf(d3dDevice,PrimitiveType.LineList,ptr/2);
                    cbuf=(CustomVertex.PositionNormalColored[])VBuf.Lock(0,0);
                    ptr=0;
                }
                int f1=VertNetwork[i],f2=VertNetwork[i+2];
                if(BPln[f1] || BPln[f2]) continue;
                Vector3 p1=StFaces[f1].Coords3D[VertNetwork[i+1]/dim];
                Vector3 p2=StFaces[f2].Coords3D[VertNetwork[i+3]/dim];
                if(p1.Z!=StkMesh.WRONGPT && p2.Z!=StkMesh.WRONGPT) {
                    cbuf[ptr++]=new CustomVertex.PositionNormalColored(p1,new Vector3(0,0,1),-0x7f7f80);
                    cbuf[ptr++]=new CustomVertex.PositionNormalColored(p2,new Vector3(0,0,1),-0x7f7f80);
                }
            }
#endif            
            SendVBuf(d3dDevice,PrimitiveType.LineList,ptr/2);
        }
        public bool GetBounds(out RPoint min,out RPoint max) {
            min=new RPoint(-1.5,-1.5,-1.5);
            max=new RPoint(1.5,1.5,1.5);
            return true;
        }

        public void CleanUp() {
            Dispose();
        }

        public bool NeedLightning {
            get { return true; }
        }

        public int FindSticker(double x,double y,int mode) {
            double zmin=double.MaxValue;
            int res=-1;
            short[] fld=Cube.Field;
            for(int ns=0;ns<NStk;ns++) {
                StkMesh s=Stks[ns];
                if(mode==FIND_STICKER_SELECTED && (fld[ns]&0x8000)==0) continue;
                if(mode==FIND_STICKER_CORNER && s.Base.MinBDim!=0) continue;
                if(mode==FIND_STICKER_ANY && (ShowRank&(1<<Math.Min(s.Base.Rank,11)))==0) continue;
                if(BPln[s.NFace]) continue;
                double z=s.CheckRay(x,y,zmin);
                if(z!=double.MaxValue && z<zmin) {
                    zmin=z; res=ns;
                }
            }
            return res;
        }

        public double[] FindFace(double x,double y,out int nf) {
            double zmin=double.MaxValue;
            double[] res=null;
            nf=-1;
            for(int ns=0;ns<NF;ns++) {
                if(BPln[ns]) continue;
                double[] r=StFaces[ns].FindPoint(x,y,ref zmin);
                if(r!=null) { res=r; nf=ns; }
            }
            return res;
        }

        internal void SetWhiteColor(int nf) {
            throw new NotImplementedException();
        }
    }
}
