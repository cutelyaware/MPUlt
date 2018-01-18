using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace _3dedit{
	public class DXUtils{


		public static Vector3 Minus(Vector3 v1, Vector3 v2){
			return Vector3.Subtract(v1,v2);
		}
		public static Vector3 Mul(Vector3 v1, float v2) {
			return Vector3.Multiply(v1,v2);
		}
		public static void MulE(ref Vector3 v1, float v2) {
			v1.Multiply(v2);
		}
		public static void MinusE(ref Vector3 v1, ref Vector3 v2) {
			v1.Subtract(v2);
		}
		public static void PlusE(ref Vector3 v1, ref Vector3 v2) {
			v1.Add(v2);
		}
		public static Vector3 Center(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3){
			Vector3 ret = new Vector3();
			ret.X=(v1.X+v2.X+v3.X)/3;
			ret.Y=(v1.Y+v2.Y+v3.Y)/3;
			ret.Z=(v1.Z+v2.Z+v3.Z)/3;
			return ret;
		}
		public static float Dist(ref Vector3 v1, ref Vector3 v2) {
			return (float)Math.Sqrt(Math.Pow(v1.X-v2.X,2.0)+
				Math.Pow(v1.Y-v2.Y,2.0)+
				Math.Pow(v1.Z-v2.Z,2.0));
		}
		public static float Perimeter(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3){
			return DXUtils.Dist(ref v1, ref v2) + DXUtils.Dist(ref v2, ref v3)
				+ DXUtils.Dist(ref v1, ref v3);
		}
		public static RPoint D3DVECTOR2RPoint(Vector3 vec) {
			return new RPoint(vec.X,vec.Y,vec.Z);
		}
		public static Vector3 RPoint2D3DVECTOR(RPoint rp) {
			return (new Vector3((float)rp.X, (float)rp.Y, (float)rp.Z));
		}
		public static RPoint ApplyMatrix(RPoint pt, ref Matrix mat) {
			Vector3 vec = new Vector3((float)pt.X,(float)pt.Y,(float)pt.Z);
			Vector3 nvec = Vector3.TransformCoordinate(vec, mat);
			return new RPoint(nvec.X,nvec.Y,nvec.Z);
		}
		//-----------------------------------------------------------------------------
		// Name: IntersectTriangle()
		// Desc: Given a ray origin (orig) and direction (dir), and three vertices of
		//       of a triangle, this function returns TRUE and the interpolated texture
		//       coordinates if the ray intersects the triangle
		//-----------------------------------------------------------------------------
		public static bool IntersectTriangle( ref Vector3 orig,ref Vector3 dir, ref Vector3 v0,
			ref Vector3 v1, ref Vector3 v2,	ref float t, ref float u, ref float v ) {
			// Find vectors for two edges sharing vert0
			Vector3 edge1 = DXUtils.Minus( v1, v0);
			Vector3 edge2 = DXUtils.Minus( v2, v0);

			// Begin calculating determinant - also used to calculate U parameter
			Vector3 pvec = Vector3.Cross(dir, edge2);

			// If determinant is near zero, ray lies in plane of triangle
			float det = Vector3.Dot( edge1, pvec );
			if( det < 0.0001f )
				return false;

			// Calculate distance from vert0 to ray origin
			Vector3 tvec = DXUtils.Minus(orig, v0);

			// Calculate U parameter and test bounds
			u = Vector3.Dot( tvec, pvec );
			if( u < 0.0f || u > det )
				return false;

			// Prepare to test V parameter
			Vector3 qvec = Vector3.Cross(tvec, edge1 );

			// Calculate V parameter and test bounds
			v = Vector3.Dot( dir, qvec );
			if( v < 0.0f || u + v > det )
				return false;

			// Calculate t, scale parameters, ray intersects triangle
			t = Vector3.Dot( edge2, qvec );
			float fInvDet = 1.0f / det;
			t *= fInvDet;
			u *= fInvDet;
			v *= fInvDet;
			return true;
		}
		static public Microsoft.DirectX.Matrix CTrans2Matrix(CTrans ct) {
			Quaternion qRotation = new Quaternion();
			qRotation.X = (float)-ct.R.X; qRotation.Y = (float)ct.R.Y; qRotation.Z = (float)ct.R.Z;
			qRotation.W = (float)-ct.R.R;
			Microsoft.DirectX.Matrix M = Microsoft.DirectX.Matrix.RotationQuaternion(qRotation);
			M.M41 = (float)-ct.T.X; M.M42 = (float)ct.T.Y; M.M43 = (float)ct.T.Z;
			return M;
		}
		static public CTrans Matrix2CTrans(Microsoft.DirectX.Matrix M) {
			RPoint T = new RPoint(
				-M.M41,
				M.M42,
				M.M43 );
			M.M41=M.M42=M.M43=0;
			Quaternion qRotation = Quaternion.RotationMatrix( M);
			QPoint R = new QPoint(
				-qRotation.X,
				qRotation.Y,
				qRotation.Z,
				-qRotation.W );
			return new CTrans(T,R);
		}
		public static void SetPoint(ref Vector3 v, int x, int y , int z) {
			v.X = x;
			v.Y = y;
			v.Z = z;
		}
	}
}

//EOF