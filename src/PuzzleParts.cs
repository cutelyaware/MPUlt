using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;
using System.IO;

namespace _3dedit {
    class PBaseAxis {
        internal int Id;
        internal double[] Dir;
        internal int NLayers;   // number of rotational layers (for GUI).
        internal double[] Cut;   // in increasing order, zero is duplicated
        internal int FixedMask;
        internal int[][] Layers;
        internal PBaseTwist[] Twists;
        internal int NPrimaryTwists;

        internal PBaseAxis(double[] dir) {
            Dir=dir;
            FixedMask=0;
        }


        internal int Remask(int mask) {
            return mask&~FixedMask;
        }

        internal int ReverseMask(int ms) {
            int r=0;
            for(int i=0;i<NLayers;i++) {
                r=(r<<1)|(ms&1);
                ms>>=1;
            }
            return r;
        }

        static int CmpDouble(double x,double y) {
            return x<y ? 1 : x==y ? 0 : -1;
        }

        internal void AdjustCuts() {
            Array.Sort<double>(Cut,CmpDouble);
            NLayers=Cut.Length+1;
            FixedMask=0;
        }

        internal void ExpandPrimaryTwists() {
            const int MaxNTwists=256;
            NPrimaryTwists=Twists.Length;
            PBaseTwist[] twst=new PBaseTwist[MaxNTwists];
            int q=0;
            foreach(PBaseTwist tw in Twists) twst[q++]=tw;
            for(int p=0;p<q;p++) {
                double[] v=twst[p].Dir;
                for(int i=0;i<q;i++) {
                    double[] w=PGeom.ApplyTwist(twst[i].Dir,v);
                    int k;
                    bool qr;
                    for(k=0;k<q;k++) {
                        if(PGeom.TwistsEqual(w,twst[k].Dir,out qr)) break;
                    }
                    if(k==q) {
                        if(q==MaxNTwists) throw new Exception("Too many planes");
                        twst[q]=new PBaseTwist(w);
                        q++;
                    }
                }
            }
            Twists=new PBaseTwist[q];
            for(int i=0;i<q;i++) Twists[i]=twst[i];
        }

        internal void DebugPrint(TextWriter tw) {
            tw.WriteLine("Base Axis {0}",Id);
            tw.Write("  Dir:");
            PuzzleStructure.PrintVec(Dir,tw,false);
            tw.WriteLine();
            tw.Write("  NLayers: {0}",NLayers);
            tw.Write("  FixedMask: {0}",FixedMask);
            tw.Write("  Cut:");
            PuzzleStructure.PrintVec(Cut,tw,false);
            tw.WriteLine();
            /*
            for(int i=0;i<NLayers;i++){
                if(Layers[i]!=null){
                    tw.Write("  Layer {0}:",i);
                    PuzzleStructure.PrintIArr(Layers[i],tw);
                    tw.WriteLine();
                }
            }*/
            tw.WriteLine("  Twists: {0}",Twists.Length);
            /*
            foreach(PBaseTwist tww in Twists){
                tww.DebugPrint(tw);
            }*/
        }
        internal int GetRank(int lv) {
            if(lv>=NLayers-1 || Cut[lv]<0) {
                for(int i=lv;--i>=0;) if(Cut[i]>=0) return lv-1-i;
                return lv;
            } else if(Cut[lv]==0) return 0;
            else {
                for(int i=lv+1;i<NLayers-1;i++) if(Cut[i]<=0) return i-lv;
                return NLayers-1-lv;
            }
        }

    }
    class PBaseTwist {
        internal double[] Dir;  // 2*dim.
        internal int Order;
        internal int[][] Map;   // sorting of stickers in layers of base axis 
        internal int[][] InvMap;

        internal PBaseTwist(double[] dir) {
            Dir=dir;
            Order=PGeom.GetOrder(Dir);
        }
        internal int ReAngle(int angle) {
            angle%=Order;
            if(angle<0) angle+=Order;
            return angle;
        }

        internal int NormAngle(int angle) {
            angle%=Order;
            if(angle<0) angle+=Order;
            if(angle>Order/2) angle-=Order;
            return angle;
        }


        internal void DebugPrint(TextWriter tw) {
            tw.Write("  Twist");
            PuzzleStructure.PrintVec(Dir,tw,true);
            tw.WriteLine("    Order={0}",Order);
            tw.WriteLine("    Map:");
            for(int i=0;i<Map.Length;i++) {
                if(Map[i]!=null) {
                    tw.Write("      Level {0}:",i);
                    PuzzleStructure.PrintIArr(Map[i],tw);
                    tw.WriteLine();
                }
            }
            tw.WriteLine("    InvMap:");
            for(int i=0;i<InvMap.Length;i++) {
                if(InvMap[i]!=null) {
                    tw.Write("      Level {0}:",i);
                    PuzzleStructure.PrintIArr(InvMap[i],tw);
                    tw.WriteLine();
                }
            }
        }
    }

    class PBaseFace {
        internal int Id;
        internal double[] Pole;
        internal double[][] FPoles;
        internal int NCutAxes;
        internal int[] CutAxes;
        internal int[] AxisLayers;  // for all axes; -1 => from CutAxes
        internal int NStickers;
        internal byte[,] StickerMask; // NStickers,NCutAxes
        internal PMesh[] StickerMesh;
        internal PMesh FaceMesh;
        internal ArrayList SMatrices; 

        internal PBaseFace(double[] pole) {
            Pole=pole;
            SMatrices=new ArrayList();
            SMatrices.Add(PGeom.MatrixIdentity(pole.Length));
        }

        internal void SetStickers(LMesh M,CutNetwork CN,PAxis[] Axes,double[][]fctrs) {
            int dim=Pole.Length;
            NCutAxes=0;
            int nax=Axes.Length;
            for(int i=0;i<nax;i++) if(AxisLayers[i]<0) NCutAxes++;
            CutAxes=new int[NCutAxes];
            int d=0;
            int rnk=0;
            for(int u=0;u<nax;u++) {
                if(AxisLayers[u]<0) CutAxes[d++]=u;
                else rnk+=Axes[u].Base.GetRank(AxisLayers[u]);
            }

            NStickers=CN.Nodes.Length;
            StickerMask=new byte[NStickers,NCutAxes];
            StickerMesh=new PMesh[NStickers];
            int nstk=0;
            for(int i=0;i<NStickers;i++) {
                PMesh xx=CN.GetPMesh(i);
                if(fctrs!=null) {
                    bool qg=false;
                    foreach(double[] p in fctrs) {
                        if(PGeom.VertEqual(p,xx.Ctr)) {
                            qg=true;
                            break;
                        }
                    }
                    if(!qg) continue;
                }

                xx.FCtr=Pole;
                double[] ctr=xx.GetMCtr();
                int rnk1=rnk;
                for(int j=0;j<NCutAxes;j++) {
                    PAxis ax=Axes[CutAxes[j]];
                    double[] h=ax.Dir;
                    double lh=PGeom.DotProd(h,h);
                    double s=0;
                    for(int k=0;k<dim;k++) s+=ctr[k]*h[k];
                    s/=lh;
                    int lv=ax.Base.NLayers-1;
                    for(int g=0;g<lv;g++) {
                        if(s>ax.Base.Cut[g]) { lv=g; break; }
                    }
                    rnk1+=ax.Base.GetRank(lv);
                    StickerMask[nstk,j]=(byte)lv;
                }
                xx.Rank=rnk1;
                StickerMesh[nstk++]=xx;
            }
            if(nstk!=NStickers) {
                PMesh[] stkm=new PMesh[nstk];
                byte[,] stkmsk=new byte[nstk,NCutAxes];
                for(int i=0;i<nstk;i++) {
                    stkm[i]=StickerMesh[i];
                    for(int j=0;j<NCutAxes;j++) stkmsk[i,j]=StickerMask[i,j];
                }
                StickerMask=stkmsk;
                StickerMesh=stkm;
                NStickers=nstk;
            }
        }

        internal int MinRank() {
            int res=int.MaxValue;
            foreach(PMesh p in StickerMesh) res=Math.Min(res,p.Rank);
            return res;
        }
        internal void SubRank(int r) {
            foreach(PMesh p in StickerMesh) p.Rank-=r;
        }
        internal int FindByMask(int[] StkMask) {
            for(int i=0;i<NStickers;i++) {
                for(int j=0;j<NCutAxes;j++) {
                    if(StickerMask[i,j]!=StkMask[j]) goto _1;
                }
                return i;
_1: ;
            }
            throw new Exception("Can't find sticker by mask");
        }

        internal void DebugPrint(TextWriter tw) {
            tw.WriteLine("Base Face {0}:",Id);
            tw.Write("  Pole:");
            PuzzleStructure.PrintVec(Pole,tw,false);
            tw.WriteLine();
            tw.WriteLine("  NCutAxes: {0}",NCutAxes);
            tw.Write("  CutAxes:");
            PuzzleStructure.PrintIArr(CutAxes,tw);
            tw.WriteLine();
            /*
            tw.Write("  AxesLayers:");
            PuzzleStructure.PrintIArr(AxisLayers,tw);
            tw.WriteLine();*/
            tw.WriteLine("  NStickers: {0}",NStickers);
            for(int i=0;i<NStickers;i++){
                tw.Write("Sticker {0}: NV={1}, NE={2}, NF={3}, Ctr=",
                    i,StickerMesh[i].NV,StickerMesh[i].NE,StickerMesh[i].NF);
                PuzzleStructure.PrintVec(StickerMesh[i].Ctr,tw,false);
                tw.Write(" Mask:");
                for(int j=0;j<NCutAxes;j++) tw.Write(" {0}",StickerMask[i,j]);
                tw.WriteLine();
            }
        }

        internal void AddSMatrix(double[,] mf,double[,] p) {
            // Pole*mf=Pole*p => Pole*(mf*p')=Pole
            
            double[,] mr=PGeom.MatrixMulInv(mf,p);
            foreach(double[,] m in SMatrices) {
                if(PGeom.MatrixEqual(m,mr)) return;
            }
            SMatrices.Add(mr);
        }

        internal void CloseSMatrixSet() {
            if(Pole.Length==4) {
                PGeom.CloseMatrixSet(SMatrices);
            }
        }
    }

    class PAxis {
        internal int Id;
        internal PBaseAxis Base;
        internal double[,] Matrix;
        internal double[] Dir;
        internal int[][] Layers; // actual stickers by layers
        internal double[][] Twists;

        internal PAxis(PBaseAxis bas) {
            Base=bas;
            Dir=bas.Dir;
            int dim=Dir.Length;
            Matrix=PGeom.MatrixIdentity(dim);
            int ntw=bas.Twists.Length;
            Twists=new double[ntw][];
            for(int i=0;i<ntw;i++) Twists[i]=bas.Twists[i].Dir;
        }

        internal PAxis(PAxis src,double []tw) {
            Base=src.Base;
            Dir=PGeom.ApplyTwist(tw,src.Dir);
            Matrix=PGeom.ApplyTwist(tw,src.Matrix);
            int ntw=src.Twists.Length;
            Twists=new double[ntw][];
            for(int i=0;i<ntw;i++) Twists[i]=PGeom.ApplyTwist(tw,src.Twists[i]);
        }

        internal int FindTwist(double[] p,double[,] matr,out bool qrev) {
            double[] q=PGeom.ApplyMatrixToTwist(matr,p);
            qrev=false;
            for(int i=0;i<Twists.Length;i++) {
                if(PGeom.TwistsEqual(q,Twists[i],out qrev)) return i;
            }
            throw new Exception("Can't fing twist");
        }

        internal void DebugPrint(TextWriter tw) {
            tw.WriteLine("Axis {0}:",Id);
            tw.WriteLine("  Base: {0}",Base.Id);
            tw.Write("  Dir:");
            PuzzleStructure.PrintVec(Dir,tw,false);
            tw.WriteLine();
/*
            for(int i=0;i<Layers.Length;i++){
                if(Layers[i]!=null){
                    tw.Write("  Layer {0}:",i);
                    PuzzleStructure.PrintIArr(Layers[i],tw);
                    tw.WriteLine();
                }
            }
            
            for(int i=0;i<Twists.Length;i++){
                tw.Write("  Twist {0}:",i);
                PuzzleStructure.PrintVec(Twists[i],tw,true);
                tw.WriteLine();                
            }*/
        }

    }
    class PFace {
        internal int Id;
        internal PBaseFace Base;
        internal double[,] Matrix;
        internal double[] Pole;

        internal int[] CutAxes;
        internal int FirstSticker;
        internal int RefAxis;

        internal PFace(PBaseFace bas) {
            Base=bas;
            Pole=bas.Pole;
            int dim=Pole.Length;
            Matrix=PGeom.MatrixIdentity(dim);
            RefAxis=0;
        }
        internal PFace(PFace src,double[] tw) {
            Base=src.Base;
            Pole=PGeom.ApplyTwist(tw,src.Pole);
            Matrix=PGeom.ApplyTwist(tw,src.Matrix);
        }

        internal void DebugPrint(TextWriter tw) {
            tw.WriteLine("Face {0}:",Id);
            tw.WriteLine("  Base: {0}",Base.Id);
            tw.Write("  Pole:");
            PuzzleStructure.PrintVec(Pole,tw,false);
            tw.WriteLine();
            tw.Write("  CutAxes:");
            PuzzleStructure.PrintIArr(CutAxes,tw);
            tw.WriteLine();
            tw.WriteLine("  First Sticker: {0}",FirstSticker);
        }
    }
    class PMesh {
        internal int PDim;
        internal int MinBDim;
        internal int Rank;
        internal int NV,NE,NF;
        internal double[] Ctr,FCtr;
        internal double[] Coords;
        internal int[] Edges;  // 2*NE
        internal int[] Faces;  // 3*NF

        internal PMesh(LMesh m) {
            Coords=m.pts;
            Edges=m.edges;
            Faces=m.faces;
            NV=m.npts;
            NE=m.nedges;
            NF=m.nfaces;
            PDim=m.pdim;
            Ctr=m.ctr;
            MinBDim=m.m_minBDim;
        }
        internal double[] GetMCtr() {
            double[] res=new double[PDim];
            for(int i=0;i<NV*PDim;i++) res[i%PDim]+=Coords[i]/NV;
            return res;         
        }
    }

    class PGeom {
        internal static int GetOrder(double[] tw) {
            double la=0,lb=0,pr=0;
            int d=tw.Length/2;
            for(int i=0;i<d;i++) {
                la+=tw[i]*tw[i];
                lb+=tw[i+d]*tw[i+d];
                pr+=tw[i]*tw[i+d];
            }
            double a=Math.Acos(Math.Min(1,pr/Math.Sqrt(la*lb)));
            if(a!=0) a=Math.PI/a;
            int r=(int)Math.Round(a);
            if(r==0 || Math.Abs(a-r)>1e-4) throw new Exception("Wrong twist order "+a);
            return r;
        }

        internal static double[] ApplyTwist(double[] mov,double[] vec) {
            int d=mov.Length/2;
            int d1=vec.Length;
            double[] res=(double[])vec.Clone();
            for(int k=0;k<d1;k+=d) {
                for(int k1=0;k1<=d;k1+=d) {
                    double sa=0,sb=0;
                    for(int i=0;i<d;i++) {
                        sa+=mov[k1+i]*mov[k1+i];
                        sb+=mov[k1+i]*res[k+i];
                    }
                    double s=2*sb/sa;
                    for(int i=0;i<d;i++) res[k+i]=mov[k1+i]*s-res[k+i];
                }
            }
            return res;
        }


        internal static bool TwistsEqual(double[] v,double[] w,out bool qr) {
            qr=true;
            bool qp=true,qm=true;
            double vl=VLength2(v);
            double wl=VLength2(w);

            int d=v.Length/2;
            for(int i=1;i<d;i++) {
                for(int j=0;j<i;j++) {
                    double p=(v[i]*v[j+d]-v[j]*v[i+d])/vl;
                    double q=(w[i]*w[j+d]-w[j]*w[i+d])/wl;
                    if(qp && Math.Abs(p-q)>1e-3) qp=false;
                    if(qm && Math.Abs(p+q)>1e-3) qm=false;
                    if(!qp && !qm) return false;
                }
            }
            qr=qm;
            return true;
        }
        internal static double VLength(double[] v) {
            double r=0;
            for(int i=0;i<v.Length;i++) r+=v[i]*v[i];
            return Math.Sqrt(r);
        }
        static double VLength2(double[] v) {
            double r=0,r1=0;
            int l=v.Length/2;
            for(int i=0;i<l;i++) r+=v[i]*v[i];
            for(int i=l;i<2*l;i++) r1+=v[i]*v[i];
            return Math.Sqrt(r*r1);
        }


        internal static double[,] MatrixIdentity(int dim) {
            double[,] M=new double[dim,dim];
            for(int i=0;i<dim;i++) M[i,i]=1;
            return M;
        }

        internal static bool VertEqual(double[] v,double[] p) {
            int dim=v.Length;
            for(int i=0;i<dim;i++) if(Math.Abs(v[i]-p[i])>0.001) return false;
            return true;
        }

        internal static double[,] ApplyTwist(double[] tw,double[,] matr) {
            int dim=tw.Length/2;
            double[,] res=new double[dim,dim];
            double[] m=new double[dim];
            for(int i=0;i<dim;i++) {
                for(int j=0;j<dim;j++) m[j]=matr[i,j];
                double[] mm=ApplyTwist(tw,m);
                for(int j=0;j<dim;j++) res[i,j]=mm[j];
            }
            return res;
        }

        internal static bool AxisEqual(double[] v,double[] p,out bool qr) {
            bool qp=true,qm=true;
            int dim=v.Length;
            for(int i=0;(qp||qm)&&i<dim;i++) {
                if(qp && Math.Abs(v[i]-p[i])>0.001) qp=false;
                if(qm && Math.Abs(v[i]+p[i])>0.001) qm=false;
            }
            qr=qm;
            return qp||qm;
        }

        internal static double Dist2(double[] a,double[] b) {
            double r=0;
            for(int i=0;i<a.Length;i++) r+=(a[i]-b[i])*(a[i]-b[i]);
            return r;
        }

        internal static double Dist2Rev(double[] a,double[] b) {
            double r=0;
            for(int i=0;i<a.Length;i++) r+=(a[i]+b[i])*(a[i]+b[i]);
            return r;
        }

        internal static double[] ApplyMatrix(double[,] matr,double[] v) {
            int d=v.Length;
            double[] r=new double[d];
            for(int i=0;i<d;i++) {
                double h=0;
                for(int j=0;j<d;j++) h+=matr[j,i]*v[j];
                r[i]=h;
            }
            return r;
        }

        internal static double[] ApplyInvMatrix(double[,] matr,double[] v) {
            int d=v.Length;
            double[] r=new double[d];
            for(int i=0;i<d;i++) {
                double h=0;
                for(int j=0;j<d;j++) h+=matr[i,j]*v[j];
                r[i]=h;
            }
            return r;
        }

        internal static double[] ApplyMatrixToTwist(double[,] matr,double[] v) {
            int d=v.Length/2;
            double[] r=new double[2*d];
            for(int i=0;i<d;i++) {
                double h=0;
                for(int j=0;j<d;j++) h+=matr[j,i]*v[j];
                r[i]=h;
                h=0;
                for(int j=0;j<d;j++) h+=matr[j,i]*v[j+d];
                r[i+d]=h;
            }
            return r;
        }
        internal static double[,] GetMatrixForTwist(double[] rtw,double ang) {
            double c0=Math.Cos(ang),s0=Math.Sin(ang);
            int d=rtw.Length/2;
            double[,]m=new double[d,d];
            for(int i=0;i<d;i++) {
                for(int j=0;j<d;j++) {
                    m[i,j]=(i==j?1:0)+(rtw[i]*rtw[j]+rtw[i+d]*rtw[j+d])*(c0-1)+(rtw[i]*rtw[j+d]-rtw[i+d]*rtw[j])*s0;
                }
            }
            return m;
        }

        internal static double DotProd(double[] a,double[] b) {
            double r=0;
            for(int i=0;i<a.Length;i++) r+=a[i]*b[i];
            return r;
        }

        internal static void CloseMatrixSet(ArrayList S) {
            double[][,] mx=new double[8192][,];
            int p=0,q=0;
            foreach(double[,] a in S) mx[q++]=a;
            while(p<q) {
                double[,] m=mx[p++];
                for(int i=0;i<q;i++) {
                    double[,] m1=MatrixMul(m,mx[i]);
                    for(int j=0;j<q;j++) {
                        if(MatrixEqual(m1,mx[j])) goto _1;
                    }
                    if(q==mx.Length) throw new Exception("Too many matrices");
                    mx[q++]=m1; S.Add(m1);
_1: ;
                }
            }
        }

        internal static double[,] MatrixMul(double[,] m,double[,] p) {
            int d=m.GetLength(0);
            double[,] res=new double[d,d];
            for(int i=0;i<d;i++) {
                for(int j=0;j<d;j++) {
                    double s=0;
                    for(int k=0;k<d;k++) s+=m[i,k]*p[k,j];
                    res[i,j]=s;
                }
            }
            return res;
        }
        internal static double[,] MatrixMulInv(double[,] m,double[,] p) {  // m*p'
            int d=m.GetLength(0);
            double[,] res=new double[d,d];
            for(int i=0;i<d;i++) {
                for(int j=0;j<d;j++) {
                    double s=0;
                    for(int k=0;k<d;k++) s+=m[i,k]*p[j,k];
                    res[i,j]=s;
                }
            }
            return res;
        }
        internal static double[,] MatrixMulInv2(double[,] m,double[,] p) {  // m'*p
            int d=m.GetLength(0);
            double[,] res=new double[d,d];
            for(int i=0;i<d;i++) {
                for(int j=0;j<d;j++) {
                    double s=0;
                    for(int k=0;k<d;k++) s+=m[k,i]*p[k,j];
                    res[i,j]=s;
                }
            }
            return res;
        }

        internal static bool MatrixEqual(double[,] m,double[,] p) {
            int d=m.GetLength(0);
            for(int i=0;i<d;i++) {
                for(int j=0;j<d;j++) {
                    if(Math.Abs(m[i,j]-p[i,j])>1e-3) return false;
                }
            }
            return true;
        }
    }
}
