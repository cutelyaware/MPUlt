namespace _3dedit {
	using System;

	public sealed class MyMath {
		public static double pyth(double a,double b){ return Math.Sqrt(a*a+b*b); }
		public static double pyth(double a,double b,double c){ return Math.Sqrt(a*a+b*b+c*c); }
		public static double sqr(double v) { return v*v; }
		public static double sum2(double a, double b) { return a*a+b*b; }
		public static double sum2(double a, double b, double c) { return a*a+b*b+c*c; }
		public static int Round(double x){
			return (int)Math.Round(x);
			//return (int)(x<0 ? x-0.5 : x+0.5);
		}
		public static double fmax(double a,double b){ return a<b ? b : a; }
		public static double fmin(double a,double b){ return a>b ? b : a; }
		public static int D2I(double v) {
			return (int)Math.Round(v);
		}
		public static double NextDouble(double d) {
			if( Double.MaxValue == d )
				return d;
			double e = Double.Epsilon;
			double de = d + e;
			while( d == de ) {
				e += e;
				de = d + e;
			}
			return de;
		}
		public static int floor(double x){
			return (int)(Math.Floor(x));
		}
		// arctan: max.error=3e-5
		const double atan_c0=2*0.9998679561;
		const double atan_c1=-2*0.3303333136;
		const double atan_c2=2*0.1802564362;
		const double atan_c3=-2*0.08524245635;
		const double atan_c4=2*0.02084954100;

		public static double Atan(double x){  // result in (-pi/2..pi/2)
			x/=Math.Sqrt(1+x*x);
			double s=x*x;
			return x*(atan_c0+s*(atan_c1+s*(atan_c2+s*(atan_c3+s*atan_c4))));
		}
		public static double Atan2(double y,double x){ // result in [-pi,pi)
			double r=Math.Sqrt(y*y+x*x);
			if(r==0) return 0;
			double a=0,b=0;
			if(y<=0){
				a=-0.5*Math.PI; b=-x/(y-r);
			}else{
				a=0.5*Math.PI; b=-x/(y+r);
			}
			double s=b*b;
			return a+b*(atan_c0+s*(atan_c1+s*(atan_c2+s*(atan_c3+s*atan_c4))));
		}
		// angle (0,0) for vector (0,0,-1); (pi/2,0) for (1,0,0); (*,pi/2) for (0,1,0)
		public static void ToSphere(double x,double y,double z,out double R,out double U,out double V){
			double w=x*x+z*z;
			if(w==0){ 
				U=0; R=Math.Abs(y); V=Math.PI*0.5*Math.Sign(y);
				return;
			}
			R=Math.Sqrt(w+y*y);
			double r=Math.Sqrt(w);

			double a=0,u=0;
			if(x<=0){ u=z/(x-r); a=-0.5*Math.PI; }
			else{ u=z/(x+r); a=0.5*Math.PI; }
			double s=u*u;
			U=a+u*(atan_c0+s*(atan_c1+s*(atan_c2+s*(atan_c3+s*atan_c4))));

			double v=y/(R+r);
			s=v*v;
			V=v*(atan_c0+s*(atan_c1+s*(atan_c2+s*(atan_c3+s*atan_c4))));
		}

	}
}

// EOF