using System;

namespace _3dedit{
	// quaternion
	public struct QPoint {
		public static readonly QPoint Zero = new QPoint(RPoint.Zero, 0.0);
		public static readonly QPoint Ident = new QPoint(RPoint.Zero, 1.0);
		//
		public RPoint PT;
		//
		public double X { get { return PT.X; } }
		public double Y { get { return PT.Y; } }
		public double Z { get { return PT.Z; } }
		// real part
		public double R;
		//
		public QPoint(RPoint pt, double r) {
			PT = pt; R = r;
		}
		public QPoint(double x, double y, double z, double r) {
			PT.X = x; PT.Y = y; PT.Z = z; R = r;
		}
		//
		public override bool Equals(object x) {
			return x is QPoint && this.Equals((QPoint)x);
		}
		public override int GetHashCode() {
			return this.PT.GetHashCode() ^ this.R.GetHashCode();
		}
		public override string ToString() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder("#<QPoint ");
			const string fmt = "0.###;-0.###;0.#";
			sb.Append(X.ToString(fmt));
			sb.Append(", ");
			sb.Append(Y.ToString(fmt));
			sb.Append(", ");
			sb.Append(Z.ToString(fmt));
			sb.Append(", ");
			sb.Append(R.ToString(fmt));
			sb.Append('>');
			return sb.ToString();
		}
		//
		public bool Equals(QPoint qpt) {
			return this == qpt;
		}
		public static bool operator ==(QPoint qa, QPoint qb) {
			return qa.PT == qb.PT && qa.R == qb.R;
		}
		public static bool operator !=(QPoint qa, QPoint qb) {
			return qa.PT != qb.PT || qa.R != qb.R;
		}
		// destructive ops
		public void Add(ref QPoint q) {
			PT.Add(ref q.PT);
			R += q.R;
		}
		public void Sub(ref QPoint q) {
			PT.Sub(ref q.PT);
			R -= q.R;
		}
		public void Mul(double v) {
			PT.Mul(v);
			R *= v;
		}
		public void Div(double v) {
			PT.Div(v);
			R /= v;
		}
		public void Norm() {
			double d = X*X + Y*Y + Z*Z + R*R;
			if( d > 0 )
				this.Div(Math.Sqrt(d));
		}
		public void Neg() {
			PT.Neg(); R = -R;
		}
		public void Conj() {
			PT.Neg();
		}
		// a=b*a
		public void LMul(ref QPoint q) {
			double r = R*q.R - X*q.X - Y*q.Y - Z*q.Z;
			double x = X*q.R + R*q.X + Z*q.Y - Y*q.Z;
			double y = Y*q.R + R*q.Y + X*q.Z - Z*q.X;
			double z = Z*q.R + R*q.Z + Y*q.X - X*q.Y;
			R = r; PT.X = x; PT.Y = y; PT.Z = z;
		}
		public void RMul(ref QPoint q) {
			QPoint qc = q;
			qc.LMul(ref this);
			this = qc;
		}
		// |b|=1
		public void RDiv(ref QPoint q) {
			QPoint qc = q;
			qc.Conj();
			qc.LMul(ref this);
			this = qc;
		}
		public void LDiv(ref QPoint q) {
			QPoint qc = q;
			qc.Conj();
			this.LMul(ref qc);
		}
		public void ToArr(double[] d) {
			PT.ToArr(d);
			d[3] = R;
		}
		public void FromArr(double[] d){
			PT.FromArr(d);
			R = d[3];
		}
	}
}

// EOF