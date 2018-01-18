using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit {
    public class S4Trans {
        int dim;
        //internal int dir3;  // 0=>center, 1=>x, 2=>y, 3=>dir3
        internal double[,] M;

        public S4Trans(int Dim) {
            dim=Dim;
            M=new double[dim,dim];
            for(int i=0;i<dim;i++) M[i,i]=1;
        }

        void Norm() {
            for(int i=0;i<dim;i++) {
                double s;
                for(int j=0;j<i;j++) {
                    s=0;
                    for(int k=0;k<dim;k++) s+=M[i,k]*M[j,k];
                    for(int k=0;k<dim;k++) M[i,k]-=s*M[j,k];
                }
                s=0;
                for(int k=0;k<dim;k++) s+=M[i,k]*M[i,k];
                s=1/Math.Sqrt(s);
                for(int k=0;k<dim;k++) M[i,k]*=s;
            }
        }
        public void Rotate(int v0,int v1,double a) {
            a=Math.Max(-0.5,Math.Min(0.5,a));
            double b=Math.Sqrt(1-a*a);
            Rotate(v0,v1,a,b);
        }
        public void Rotate(int v0,int v1,double a,double b) {
            if(v0>=dim || v1>=dim) return;
            for(int i=0;i<dim;i++) {
                double c=b*M[v0,i]+a*M[v1,i];
                M[v1,i]=b*M[v1,i]-a*M[v0,i];
                M[v0,i]=c;
            }
        }

        double[] t1=new double[3];
        public bool Apply(double[] pt,out Vector3 res,double R) {
            double x=0,y=0,z=0,w=0;
            for(int j=0;j<dim;j++){
                w+=pt[j]*M[0,j];
                x+=pt[j]*M[1,j];
                y+=pt[j]*M[2,j];
                if(dim>3) z+=pt[j]*M[3,j];
            }
            if(dim<3) z=1;
            res=new Vector3();
            if(w>R) return false;
            w=w-R;
            res.X=(float)(x/w); res.Y=(float)(y/w); res.Z=(float)(z/w);
            return true;
        }        
        public void Recenter(double []pt) {
            for(int a=1;a<=2;a++) {
                for(int i=0;i<dim;i++) {
                    M[0,i]=(M[0,i]*(2-a)-pt[i]*a)/2;
                }
                Norm();
            }
        }
        public void SwitchZ() {
            if(dim>4) {
                for(int i=0;i<dim;i++) {
                    double m=M[3,i];
                    for(int j=4;j<dim;j++) {
                        M[j-1,i]=M[j,i];
                    }
                    M[dim-1,i]=m;
                }
            }
        }
    }


    public class S4Camera:Camera {
        S4Trans Trans;
        int Dim;
        double R,R0;
        public bool m_changed;
        bool[] m_addMask;  // shift, ctrl, alt
        Microsoft.DirectX.Matrix m_ViewMatr;

        public S4Camera(int dim,double _R) {
            Init(dim,_R);
            Angle=1;
        }
        internal void Init(int dim,double rad) {
            Dim=dim; R0=rad;
            Init();
        }

        void Init() {
            m_ViewMatr=Microsoft.DirectX.Matrix.LookAtLH(new Vector3(0,0,0),new Vector3(0,0,-1),new Vector3(0,1,0));
            R=R0;
            Trans=new S4Trans(Dim);
        }

        public void SetChanged() {
            m_changed=true;
        }

        public override void TargetTo(ref Vector3 pt) {
            throw new NotImplementedException();
        }

        public void Recenter(double []pt) {
            Trans.Recenter(pt);
            m_changed=true;
        }

        public void SwitchZ() {
            Trans.SwitchZ();
            m_changed=true;
        }

        public override bool Park(ArrayList dxobj,ECameraPosition position,bool center) {
            Init();
            return true;
        }

        public override Vector3 GetLightVector() {
            return new Vector3(0.2f,0.2f,-1f);
        }

        public void SetAddMask(bool[] mask) {
            m_addMask=mask;
        }

        public override void ProcessMouseMove(S3DirectX scene,EAction action,ETarget tg,Point pt,int DX,int DY,OnAction proc) {
            /*
                        if(keys.qRightMouse){
             *              if(keys.qShift) _thisAction=EAction.ActionXYSlide; // WSlide, XY-rotate
             *              else _thisAction=EAction.ActionSlide; // WSlide, XY-rotate
             *              }else{
                        if(keys.qCtrl) _thisAction=EAction.ActionMoveTo;     // Zoom
                        else if(keys.qShift) _thisAction=EAction.ActionPan;  // WPan
                        else _thisAction=EAction.ActionRotate;               // 3D rotate
             *          }
            */



            Vector3 vec = new Vector3();
            Vector3 shft = new Vector3();
            int wid=scene.width;
            int hgt=scene.height;
            double acf=3.0/hgt; // pi/3 for half of screen

            double ddis=Math.Pow(0.5,DY*3.0/hgt);
            double wdis=Math.Pow(0.5,DX*3.0/wid);

            switch(action) {
                case EAction.ActionSlide:  // R
                case EAction.ActionXYSlide:  // Shift-R
                    Trans.Rotate(2,1,DX*2*Math.PI/wid);
                    Trans.Rotate(0,3,(double)DY*acf*Angle);
                    break;
                case EAction.ActionMoveTo:  // Ctrl-L
                    R=Math.Min(2*R0,Math.Max(0.1*R0,(R+R0)/ddis))-R0;
                    break;
                case EAction.ActionZoom:  // Ctrl-R
                    Angle=(float)Math.Min(Math.PI,Angle/ddis);
                    break;
                case EAction.ActionPan: // Shift-L
                    Trans.Rotate(0,1,(double)DX*acf*Angle);
                    Trans.Rotate(0,2,(double)DY*acf*Angle);
                    break;
                case EAction.ActionRotate: {  // L
                        if(m_addMask!=null) {
                            if(m_addMask[0]) goto case EAction.ActionPan;
                            if(m_addMask[1]) goto case EAction.ActionMoveTo;
                        }
                        double dx=(double)(DX*acf);
                        double dy=(double)(DY*acf);
                        Trans.Rotate(3,1,dx*Angle);
                        Trans.Rotate(3,2,dy*Angle);
                        break;
                    }
                case EAction.ActionClick:
                case EAction.ActionShiftClick:
                case EAction.ActionAltClick:
                case EAction.ActionCtrlClick:
                case EAction.ActionCtrlShiftClick:
                case EAction.ActionCtrlAltClick:
                case EAction.ActionRClick:
                case EAction.ActionShiftRClick:
                case EAction.ActionAltRClick:
                case EAction.ActionCtrlShiftRClick:
                case EAction.ActionCtrlRClick: {
                        double vx,vy;
                        scene.DirAndOrig(pt.X,pt.Y,out vx,out vy);
                        vec.Normalize();
                        if(proc!=null) proc(action,vx,vy,0);
                        break;
                    }
            }
            m_changed=true;
            scene.SceneChanged=true;
        }

        public override Microsoft.DirectX.Matrix GetViewMatrix() {
            return m_ViewMatr;
        }
        public override void GetDynamicZRange(float zfar_ratio,out float RenderZnear,out float RenderZfar) {
            RenderZnear=(float)(1.0/Math.Sqrt(zfar_ratio));
            RenderZfar=(float)(Math.Sqrt(zfar_ratio));
        }

        public bool Apply(double[] pt,out Vector3 res) {
            bool x=Trans.Apply(pt,out res,R);            
            return x;
        }

        internal bool AbovePlane(double[] Pole) {
            double s=0,s1=0;
            for(int i=0;i<Dim;i++) {
                s+=Pole[i]*Trans.M[0,i];
                s1+=Pole[i]*Pole[i];
            }
            return s*R>=s1;
        }

    }
}
