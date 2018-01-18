using System;
//using System.Collections;
using System.Diagnostics;
using System.IO;

namespace _3dedit {

	public class CTrans{
		// Identical
		public static readonly CTrans Ident = new CTrans();
		// shift
		protected RPoint m_T = RPoint.Zero;
		// rotate
		protected QPoint m_R = QPoint.Ident;
		//
		// TODO: make private setT
		public RPoint T {
			get { return m_T; }
			set { m_T = value; }
		}
		// TODO: make private setR
		public QPoint R {
			get { return m_R; }
			set {
				m_R = value;
				m_R.Norm();
			}
		}
		// default CTOR - Ident transformation
		public CTrans() {}
		//
		public CTrans(RPoint shiftV, QPoint rotQ) {
			this.T = shiftV;
			this.R = rotQ; //! Norm
		}
		public CTrans(CTrans ct) {
			this.m_T = ct.T;
			this.m_R = ct.R; // no Norm needed here
		}
		//
		public override int GetHashCode() {
			return this.T.GetHashCode() ^ this.R.GetHashCode();
		}
		public override bool Equals(object x) {
			return this.Equals(x as CTrans);
		}
		//
		public bool Equals(CTrans ct) {
			if( null == ct ) return false;
			return this.T.Equals(ct.T) && this.R.Equals(ct.R);
		}
		public static bool operator==(CTrans ca, CTrans cb) {
			if( null == (object)ca ) return null == (object)cb;
			return ca.Equals(cb);
		}
		public static bool operator!=(CTrans ca, CTrans cb) {
			return !(ca == cb);
		}
		// formula shortcuts
		public static CTrans operator*(CTrans ca, CTrans cb) {
			return ca.MakeSuperpose(cb);
		}
		public CTrans inv(){
			return this.MakeInverse();
		}
		//
		//
		// l=r^(-1)*(w-t)*r

		public void WtoL(ref RPoint WP, out RPoint LP) {
			QPoint Q = new QPoint(WP, 0.0);
			Q.PT.Sub(ref m_T);
			Q.RMul(ref m_R);
			Q.LDiv(ref m_R);
			LP = Q.PT;
		}
		// l=r^(-1)*(w)*r
		public void WtoL_V(ref RPoint WP, out RPoint LP) {
			QPoint Q = new QPoint(WP, 0.0);
			Q.RMul(ref m_R);
			Q.LDiv(ref m_R);
			LP = Q.PT;
		}
		// w=r*(l)*r^(-1)+t
		public void LtoW(ref RPoint LP, out RPoint WP) {
			this.LtoW_V(ref LP, out WP);
			WP.Add(ref m_T);
		}
		// l=r*(w)*r^(-1)
		public void LtoW_V(ref RPoint LP, out RPoint WP) {
			QPoint Q = new QPoint(LP, 0.0);
			Q.LMul(ref m_R);
			Q.RDiv(ref m_R);
			WP = Q.PT;
		}
		public double[,] GetMatrix(){
			double [,]M=new double[3,4];
			RPoint WX,WY,WZ,W0;
			RPoint W=new RPoint(0,0,0);
			LtoW(ref W,out W0);
			W=new RPoint(1,0,0);
			LtoW_V(ref W,out WX);
			W=new RPoint(0,1,0);
			LtoW_V(ref W,out WY);
			W=new RPoint(0,0,1);
			LtoW_V(ref W,out WZ);
			M[0,0]=WX.X; M[0,1]=WY.X; M[0,2]=WZ.X; M[0,3]=W0.X;
			M[1,0]=WX.Y; M[1,1]=WY.Y; M[1,2]=WZ.Y; M[1,3]=W0.Y;
			M[2,0]=WX.Z; M[2,1]=WY.Z; M[2,2]=WZ.Z; M[2,3]=W0.Z;
			return M;
		}
		//
		// new(L) = par(this(L))
		protected void ApplyRt(ref RPoint tr, ref QPoint rt) {
			QPoint QT = new QPoint(m_T, 0);
			QT.LMul(ref rt);
			QT.RDiv(ref rt);
			m_T = QT.PT;
			m_T.Add(ref tr);
			m_R.LMul(ref rt);
		}
		//
		// res(L) = this(B2(L))
		public CTrans MakeSuperpose(CTrans B2) {
			CTrans res = new CTrans(B2);
			res.ApplyRt(ref this.m_T, ref this.m_R);
			return res;
		}
		//
		// res(L) = CTrans(tr,rt)(this(L))
		public CTrans MakeApplyRt(ref RPoint tr, ref QPoint rt) {
			CTrans res = new CTrans(this);
			res.ApplyRt(ref tr,ref rt);
			return res;
		}
		// res=this'; L=this(res(L))=res(this(L))
		public CTrans MakeInverse(){
			QPoint QT = new QPoint(RPoint.Zero-this.T, 0);
			QT.RMul(ref this.m_R);
			QT.LDiv(ref this.m_R);
			CTrans res = new CTrans(QT.PT, this.m_R);
			res.m_R.Conj();
			return res;
		}

		// B1 = this;
#if false
		// res(L) = B1'(B2(L)), B2(L)=B1(res(L));
		public CTrans MakeRelative(CTrans B2) {
            return this.MakeInverse().MakeSuperpose(B2);
		}
		// B1 = this
		// B1(B2(L)) = B2(res(L)), res(L)=B2'(B1(B2(L)))
		public CTrans MakePullDown(CTrans B2) {
			return B2.MakeRelative(this.MakeSuperpose(B2));
		}
#endif
		//
		public static CTrans FromAxis(RPoint pt, RPoint vec, double an){
			an *= Math.PI/360.0;  // half in radians
			RPoint T = RPoint.Zero;
			RPoint C = pt;

			double lR=vec.Dist();
			double xc=Math.Cos(an),xs=Math.Sin(an);
			vec.Mul(xs/lR);
			QPoint R = new QPoint(vec,xc);
			R.Norm();
			// w=R^(-1)*(l-C)*R+C
			T.Add(ref C);
			QPoint QC = new QPoint(C,0);
			QC.LMul(ref R);
			QC.RDiv(ref R);
			T.Sub(ref QC.PT);
			return new CTrans(T,R);
		}
		public bool GetAxis(double Alpha, out RPoint pt, out RPoint vec, out double ang){
			const double CCF = 1.0;
			double lR=this.m_R.PT.Dist();
			double an=2*MyMath.Atan2(lR,this.m_R.R);
			//
			if(Math.Abs(an)<0.1){
				vec = pt = RPoint.Zero; ang = 0.0;
				return false;	//ERR: too small angle for LoadAxis
			}
			vec = m_R.PT;
			vec.Div(lR);
			//
			if(Math.Sin(an)*Math.Sin(Alpha*Math.PI/180.0)<0.0){
				an=-an;
				vec.Neg();
			}
			pt = this.m_T;

			double a=vec.SMul(ref pt); // vec.X*pt.X+vec.Y*pt.Y+vec.Z*pt.Z;
			double []w = new double[3];
			double []u = new double[3];
			w[0] = pt.X - a*vec.X;
			w[1] = pt.Y - a*vec.Y;
			w[2] = pt.Z - a*vec.Z;
			u[0]=w[1]*vec.Z-w[2]*vec.Y;
			u[1]=w[2]*vec.X-w[0]*vec.Z;
			u[2]=w[0]*vec.Y-w[1]*vec.X;
			double b=Math.Tan((Math.PI+an)/2);
			int j=0;
			if(Math.Abs(vec.X)<Math.Abs(vec.Y)) j=1;
			double cmp = (j == 0) ? vec.X : vec.Y;
			if(Math.Abs(cmp)<Math.Abs(vec.Z)) j=2;
			pt.X = (w[0]+CCF*u[0]*b)/2;
			pt.Y = (w[1]+CCF*u[1]*b)/2;
			pt.Z = (w[2]+CCF*u[2]*b)/2;
			if(j == 0)
				b = pt.X/vec.X;
			else if(j == 1)
				b = pt.Y/vec.Y;
			else
				b = pt.Z/vec.Z;
			pt.X -= b*vec.X;
			pt.Y -= b*vec.Y;
			pt.Z -= b*vec.Z;
			ang=an*180.0/Math.PI;
			return true;
		}
	}
}
// EOF
// &>>&2B 0 0 6 
