using System;
using System.Diagnostics;

namespace _3dedit {

	// regular point - 3 coordinates
	public struct RPoint {
		private static double sqr(double v) { return v*v; }
		public static readonly RPoint Zero = new RPoint(0.0, 0.0, 0.0);
		//
		public double X, Y, Z;
		//
		public RPoint(double x, double y, double z) {
			X = x; Y = y; Z = z;
		}
		public override bool Equals(object x) {
			return x is RPoint && this.Equals((RPoint)x);
		}
		//
		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}
		public override string ToString() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder("#<RPt ");
			const string fmt = "0.###;-0.###;0.#";
			sb.Append(X.ToString(fmt));
			sb.Append(", ");
			sb.Append(Y.ToString(fmt));
			sb.Append(", ");
			sb.Append(Z.ToString(fmt));
			sb.Append(">");
			return sb.ToString();
		}
		//
		public bool Equals(RPoint pt) {
			return this == pt;
		}
		public static bool operator ==(RPoint pa, RPoint pb) {
			return pa.X == pb.X && pa.Y == pb.Y && pa.Z == pb.Z;
		}
		public static bool operator !=(RPoint pa, RPoint pb) {
			return pa.X != pb.X || pa.Y != pb.Y || pa.Z != pb.Z;
		}
		//
		public static RPoint operator *(RPoint pt, double v) {
			return new RPoint(pt.X*v, pt.Y*v, pt.Z*v);
		}
		public static RPoint operator /(RPoint pt, double v) {
			return new RPoint(pt.X/v, pt.Y/v, pt.Z/v);
		}
		public static RPoint operator +(RPoint pa, RPoint pb) {
			return new RPoint(pa.X+pb.X, pa.Y+pb.Y, pa.Z+pb.Z);
		}
		public static RPoint operator -(RPoint pa, RPoint pb) {
			return new RPoint(pa.X-pb.X, pa.Y-pb.Y, pa.Z-pb.Z);
		}
		// destructive methods
		public void Add(double v) {
			X += v; Y += v; Z += v;
		}
		public void Add(ref RPoint pt) {
			X += pt.X; Y += pt.Y; Z += pt.Z;
		}
		public void Add(ref RPoint pt, double v) {
			X += pt.X*v; Y += pt.Y*v; Z += pt.Z*v;
		}
		//
		public void Sub(double v) {
			X -= v; Y -= v; Z -= v;
		}
		public void Sub(ref RPoint pt) {
			X -= pt.X; Y -= pt.Y; Z -= pt.Z;
		}
		//
		public void Mul(double v) {
			X *= v; Y *= v; Z *= v;
		}
		//
		public void Div(double v) {
			X /= v; Y /= v; Z /= v;
		}
		//
		public void Neg() {
			X = -X; Y = -Y; Z = -Z;
		}
		public void NegXZ() {
			X =- X; Z =- Z;
		}
		public void SetMin(ref RPoint pt) {
			if(pt.X < X) X = pt.X;
			if(pt.Y < Y) Y = pt.Y;
			if(pt.Z < Z) Z = pt.Z;
		}
		public void SetMax(ref RPoint pt) {
			if(pt.X > X) X = pt.X;
			if(pt.Y > Y) Y = pt.Y;
			if(pt.Z > Z) Z = pt.Z;
		}
		//
		public RPoint CrProd(ref RPoint pt) {  // res = [this, pt]
			return new RPoint(Y*pt.Z - Z*pt.Y, Z*pt.X - X*pt.Z, X*pt.Y - Y*pt.X);
		}
		public void Norm() {
			double d = this.Dist();
			if( d > 0.0 )
				this.Div(d);
		}
		// non destructive
		public double SMul(ref RPoint p) {
			return X*p.X + Y*p.Y + Z*p.Z;
		}
		public double Dist() {
			return Math.Sqrt(X*X+Y*Y+Z*Z);
		}
		public double Dist2(ref RPoint pt) {
			return sqr(X-pt.X)+sqr(Y-pt.Y)+sqr(Z-pt.Z);
		}
		public double Dist(ref RPoint pt) {
			return Math.Sqrt(this.Dist2(ref pt));
		}
		public double DdZ() {
			return this.Dist()/Math.Abs(Z);
		}
		public RPoint Ort(){
			int c=0; double a=Math.Abs(X);
			if(a>Math.Abs(Y)){ c=1; a=Math.Abs(Y); }
			if(a>Math.Abs(Z)){ c=2; a=Math.Abs(Z); }
			RPoint res;
			if(c==0) res=new RPoint(0,Z,-Y);
			else if(c==1) res=new RPoint(Z,0,-X);
			else res=new RPoint(Y,-X,0);
			res.Norm();
			return res;
		}

		public bool InBox3(double[] V) {
			return this.InVBox(V);
		}

		public void ToArr(double[] d) {
			d[0] = X; d[1] = Y; d[2] = Z;
		}
		public void FromArr(double[] d) {
			X = d[0]; Y = d[1]; Z = d[2];
		}
		//
		public void ToArr(double[] d,int ind) {
			d[ind+0] = X; d[ind+1] = Y; d[ind+2] = Z;
		}
		public void FromArr(double[] d,int ind) {
			X = d[ind+0]; Y = d[ind+1]; Z = d[ind+2];
		}
		public bool InVBox(double []VB){
			return X>=VB[0] && X<=VB[1]
				&& Y>=VB[2] && Y<=VB[3]
				&& Z>=VB[4] && Z<=VB[5];
		}
		// Part for work with transformation between local CS and raster
		// u,v on screen, z is actual, z<0
		public double GetZ(){ return this.Z; }
		public double ToProj(out double p, out double q) {  // returns z
			double z = -this.Z;
			p = this.X/z;
			q = this.Y/z;
			return -z;
		}
		public double ToCart(out double p, out double q) {  // returns z
			p = this.X;
			q = this.Y;
			return this.Z;
		}
		public static void FromProj(out RPoint pt, double u, double v, double z){
			pt.X = -u*z; pt.Y = -v*z; pt.Z = z;
		}
		public static void FromCart(out RPoint pt, double u, double v, double z){
			pt.X = u; pt.Y = v; pt.Z = z;
		}
		public bool IsZero(){
			return this == Zero;
		}
		//
		public bool InVBox(ref RPoint ptMin, ref RPoint ptMax) {
			return ptMin.X <= X && X <= ptMax.X
				&& ptMin.Y <= Y && Y <= ptMax.Y
				&& ptMin.Z <= Z && Z <= ptMax.Z;
		}
	}
	public class Axis {
		public RPoint PT;
		public RPoint VC;
		//
		public Axis(RPoint pt, RPoint vec) {
			PT = pt;
			Debug.Assert( vec.Dist() > 0 );
			VC = vec; VC.Norm();
		}
	}
}

// EOF