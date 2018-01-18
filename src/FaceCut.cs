using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace _3dedit {
    class LMesh {
        internal int pdim;
        internal double[] pts;
        internal int npts;
        internal int[] edges;
        internal int nedges;
        internal int[] faces;
        internal int nfaces;
        internal bool qsetBrd;
        internal double[] ctr;
        int m_npts;
        internal int m_minBDim;

        internal LMesh(int _pdim,bool qsetbrd) {
            pdim=_pdim;
            npts=nedges=nfaces=0;
            pts=new double[pdim*100];
            edges=new int[2*100];
            faces=new int[3*100];
            qsetBrd=qsetbrd;
            m_minBDim=int.MaxValue;
            m_npts=0;
            ctr=new double[pdim];
        }
        internal int AddPoint(double[] pt,int bb) {
            if(pts.Length<pdim*(npts+1)) {
                double[] p=new double[2*pdim*npts];
                for(int i=0;i<pdim*npts;i++) p[i]=pts[i];
                pts=p;
            }
            for(int i=0;i<pdim;i++) pts[npts*pdim+i]=pt[i];
            if(bb<m_minBDim) {
                m_npts=0;
                for(int i=0;i<pdim;i++) ctr[i]=0;
                m_minBDim=bb;
            }
            if(m_minBDim==bb) {
                m_npts++;
                for(int i=0;i<pdim;i++) ctr[i]+=pt[i];
            }
            return npts++;
        }
        internal void AddSeg(int a,int b) {
            int d=2*nedges;
            if(edges.Length<d+2) {
                int[] f=new int[d*2];
                for(int i=0;i<d;i++) f[i]=edges[i];
                edges=f;
            }
            edges[d]=a; edges[d+1]=b;
            nedges++;
        }
        internal void AddTrg(int a,int b,int c) {
            int d=3*nfaces;
            if(faces.Length<d+3) {
                int[] f=new int[d*2];
                for(int i=0;i<d;i++) f[i]=faces[i];
                faces=f;
            }
            faces[d]=a; faces[d+1]=b;
            faces[d+2]=c;
            nfaces++;
        }
        internal void CloseCtr() {
            if(m_npts!=0) for(int i=0;i<pdim;i++) ctr[i]/=m_npts;
        }
    }

    class CutNode {
        internal const int STAT_ZERO=0;
        internal const int STAT_PLUS=1;
        internal const int STAT_MINUS=-1;

        static int LastID=0;

        internal int Id;
        internal int Dim;
        internal int OpGen; // last operation number 
        internal int Status;
        internal int BrdDim; // least dimension of border containing in the sticker
        internal CutNode[] Children;
        internal CutNode ZeroNode;
        internal CutNode Pair;
        internal double[] Pole;  // correct only for PDim-1 and PDim-2
        internal double[] Ctr;

        internal bool HaveZero { get { return ZeroNode!=null; } }
        internal bool IsDivided { get { return Pair!=null; } }

        internal CutNode() { Id=LastID++; }

        internal virtual CutNode Copy(int opgen) {
            if(opgen==OpGen) return Pair;
            OpGen=opgen;
            CutNode res=new CutNode();
            res.Dim=Dim;
            res.Children=new CutNode[Children.Length];
            for(int i=0;i<Children.Length;i++) res.Children[i]=Children[i].Copy(opgen);
            Pair=res;
            return res;
        }

        internal virtual void Split(int opgen,double[] hpln,bool qpos) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            ZeroNode=Pair=null;
            int nplus=0,nminus=0;
            foreach(CutNode D in Children) {
                D.Split(opgen,hpln,qpos);
                if(D.IsDivided) nminus++;
                switch(D.Status) {
                    case STAT_MINUS: nminus++; break;
                    case STAT_PLUS: nplus++; break;
                    case STAT_ZERO: ZeroNode=D; break;
                }
            }
            if(nplus==0) {
                Status=nminus!=0 ? STAT_MINUS : STAT_ZERO;
                //                PrintLevel();
                return;
            }
            if(nminus==0) {
                Status=STAT_PLUS;
                //                PrintLevel();
                return;
            }
            CutNode[] chp=new CutNode[nplus+1];
            CutNode[] chm=new CutNode[nminus+1];

            if(ZeroNode==null) {
                if(Dim==1) {
                    if(nplus!=1 || nminus!=1) throw new Exception("Wrong length of edge");
                    PointNode p0=(PointNode)Children[0];
                    PointNode p1=(PointNode)Children[1];
                    double p=p1.Val/(p1.Val-p0.Val);
                    int n=p0.Pos.Length;
                    double[] cc=new double[n];
                    for(int i=0;i<n;i++) cc[i]=p0.Pos[i]*p+p1.Pos[i]*(1-p);
                    PointNode z=new PointNode();
                    z.Pos=cc;
                    z.Val=0;
                    ZeroNode=z;
                } else {
                    ArrayList arr=new ArrayList();
                    foreach(CutNode D in Children) {
                        if(D.Status==STAT_PLUS && D.HaveZero) arr.Add(D.ZeroNode);
                    }
                    if(Dim==2 && arr.Count!=2) throw new Exception("Wrong number of segment ends");
                    ZeroNode=new CutNode();
                    ZeroNode.Children=(CutNode[])arr.ToArray(typeof(CutNode));
                }
                ZeroNode.Dim=Dim-1;
                ZeroNode.Status=STAT_ZERO;
                ZeroNode.BrdDim=BrdDim;
                ZeroNode.Pole=IntPole(Pole,hpln);
                ZeroNode.OpGen=opgen;               
                //                if(Dim>1) ZeroNode.PrintLevel();
            }

            chp[0]=chm[0]=ZeroNode;
            int np=1,nm=1;
            foreach(CutNode D in Children) {
                if(D.IsDivided) chm[nm++]=D.Pair;
                if(D.Status==STAT_PLUS) chp[np++]=D;
                else if(D.Status==STAT_MINUS) chm[nm++]=D;
            }
            if(np!=nplus+1 || nm!=nminus+1) throw new Exception("Wrong length of children");

            Pair=new CutNode();
            Pair.Dim=Dim;
            Pair.Children=chm;
            Pair.Status=STAT_MINUS;
            Pair.ZeroNode=ZeroNode;
            Pair.BrdDim=BrdDim;
            Pair.OpGen=opgen;

            Children=chp;
            Status=STAT_PLUS;
            //            PrintLevel();
        }

        void PrintLevel() {
            string s=string.Format("Id={0}: Stat={1}, Children=[",Id,Status);
            for(int i=0;i<Children.Length;i++) {
                if(i!=0) s=s+",";
                s=s+Children[i].Id;
            }
            s=s+"]";
            if(ZeroNode!=null) s=s+", Z="+ZeroNode.Id;
            Console.WriteLine(s);
        }

        static double[] IntPole(double[] Pole,double[] hpln) {
            int dim=hpln.Length-1;
            double[] res=new double[dim];
            if(Pole==null) {
                for(int i=0;i<dim;i++) res[i]=-hpln[i];
            } else {
                double sa=0,sb=0,sc=0;
                for(int i=0;i<dim;i++) {
                    sa+=Pole[i]*Pole[i];
                    sb-=Pole[i]*hpln[i];
                    sc+=hpln[i]*hpln[i];
                }
                // (a,x-a)=(c,x-c)=0; x=p*a+q*c;
                // p*sa+q*sb=sa
                // p*sb+q*sc=sc
                double det=sa*sc-sb*sb;
                if(Math.Abs(det)<1e-10) return res;
                double p=sc*(sa-sb)/det,q=sa*(sc-sb)/det;
                for(int i=0;i<dim;i++) res[i]=Pole[i]*p-hpln[i]*q;
            }
            return res;
        }

        static internal CutNode GenCube(int dim,double V) {
            int m=1;
            for(int i=0;i<dim;i++) m*=3;
            CutNode[] u=new CutNode[m];
            for(int a=0;a<m;a++) {
                int d=0;
                for(int b=a;b>0;b/=3) if(b%3==2) d++;
                if(d==0) {
                    double[] pt=new double[dim];
                    int b=a;
                    for(int i=0;i<dim;i++) {
                        pt[i]=(b%3-0.5)*2*V;
                        b/=3;
                    }
                    u[a]=new PointNode() { Pos=pt,Dim=0 };
                } else {
                    CutNode[] ch=new CutNode[2*d];
                    int k=0;
                    for(int b=1;b<m;b*=3) {
                        if((a/b)%3==2) {
                            ch[k++]=u[a-2*b];
                            ch[k++]=u[a-b];
                        }
                    }
                    u[a]=new CutNode() { Dim=d,Children=ch };
                }
            }
            return u[m-1];
        }

        internal virtual void FillLMesh(int opgen,LMesh m) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            if(m.qsetBrd) BrdDim=Dim;
            foreach(CutNode c in Children) c.FillLMesh(opgen,m);
            if(Dim==1) {
                int a=((PointNode)Children[0]).Status;
                int b=((PointNode)Children[1]).Status;
                m.AddSeg(a,b);
            } else if(Dim==2) {
                int l=Children.Length;
                int[,] s=new int[l,2];
                for(int i=0;i<l;i++) {
                    s[i,0]=((PointNode)Children[i].Children[0]).Status;
                    s[i,1]=((PointNode)Children[i].Children[1]).Status;
                }
                for(int i=1;i<l;i++) {
                    int c=s[i-1,1];
                    int a=s[i,0],b=s[i,1];
                    if(a==c) continue;
                    if(b==c) {
                        s[i,0]=b; s[i,1]=a;
                        continue;
                    }
                    for(int k=i;k<l;k++) {
                        if(s[k,0]==c) {
                            s[i,0]=c; s[i,1]=s[k,1];
                            s[k,0]=a; s[k,1]=b;
                            break;
                        }
                        if(s[k,1]==c) {
                            s[i,0]=c; s[i,1]=s[k,0];
                            s[k,0]=a; s[k,1]=b;
                            break;
                        }
                    }
                }
                for(int i=2;i<l;i++) {
                    m.AddTrg(s[0,0],s[i-1,0],s[i,0]);
                }

            }
        }

        internal virtual void AddCenters(int opgen,ArrayList arr) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            Ctr=null;
            int nctr=0;
            foreach(CutNode c in Children) {
                c.AddCenters(opgen,arr);
                double[] r=c.Ctr;
                if(Ctr==null) Ctr=new double[r.Length];
                for(int i=0;i<r.Length;i++) Ctr[i]+=r[i];
                nctr++;
            }
            for(int i=0;i<Ctr.Length;i++) Ctr[i]/=nctr;
            if(arr!=null) arr.Add(Ctr);
        }
    }

    class PointNode: CutNode {
        const double EPS=0.0001;
        internal double[] Pos;
        internal double Val;

        internal override CutNode Copy(int opgen) {
            if(opgen==OpGen) return Pair;
            PointNode res=new PointNode();
            res.Dim=Dim;
            res.Pos=(double[])Pos.Clone();
            Pair=res;
            OpGen=opgen;
            return res;
        }
        internal override void Split(int opgen,double[] hpln,bool qpos) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            int n=Pos.Length;
            double d=hpln[n];
            for(int i=0;i<n;i++) d+=Pos[i]*hpln[i];
            if(Math.Abs(d)<EPS) d=0;
            Val=d;
            Status=d<0 ? STAT_MINUS : d==0 ? STAT_ZERO : STAT_PLUS;
        }
        internal override void FillLMesh(int opgen,LMesh m) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            if(m.qsetBrd) BrdDim=0;
            Status=m.AddPoint(Pos,BrdDim);
        }
        internal override void AddCenters(int opgen,ArrayList arr) {
            if(opgen==OpGen) return;
            OpGen=opgen;
            Ctr=(double[])Pos.Clone();
            if(arr!=null) arr.Add(Ctr);
        }
    }

    class CutNetwork {
        internal CutNode[] Nodes;
        int Dim;
        int OpGen;

        internal CutNetwork(CutNode x,int dim,int opgen) {
            Nodes=new CutNode[] { x };
            Dim=dim;
            OpGen=opgen;
        }
        internal void Split(double[] hpln,bool qpos) {
            OpGen++;
            ArrayList x1=new ArrayList();
            foreach(CutNode p in Nodes) {
                p.Split(OpGen,hpln,qpos);
                x1.Add(p);
                if(!qpos && p.IsDivided) x1.Add(p.Pair);
            }
            Nodes=(CutNode[])x1.ToArray(typeof(CutNode));
        }

        internal static double[] GetPlane(double[] pole,double cf) {
            int d=pole.Length;
            double[] res=new double[d+1];
            double s=0;
            for(int i=0;i<d;i++) {
                res[i]=-pole[i];
                s+=pole[i]*pole[i];
            }
            res[d]=s*cf;
            return res;
        }
        internal PMesh GetPMesh(int n) {
            LMesh m=new LMesh(Dim,false);
            Nodes[n].FillLMesh(++OpGen,m);
            m.CloseCtr();
            return new PMesh(m);
        }

        internal double[][] GetCtrs() {
            ArrayList arr=new ArrayList();
            for(int i=0;i<Nodes.Length;i++) {
                Nodes[i].AddCenters(++OpGen,arr);
            }
            return (double[][])arr.ToArray(typeof(double[]));
        }

        internal void PrintStat() {
            for(int i=0;i<Nodes.Length;i++) {
                LMesh m=new LMesh(Dim,false);
                Nodes[i].FillLMesh(++OpGen,m);
                Console.WriteLine("Item {0}: NVert={1}, NEdge={2}",i,m.npts,m.nfaces);
            }
        }
    }
}
