﻿//------------------------------------------------------------------------------
//	<auto-generated>
//		This code was generated from a template.
//		Manual changes to this file will be overwritten if the code is regenerated.
//	</auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using VectorF = System.Numerics.Vector<float>;

namespace PhotoSauce.MagicScaler.Transforms
{
	internal sealed partial class Convolver4ChanVector : IConvolver
	{
		private const uint channels = 4;
		private const uint vector4Count = 4;

		public static readonly Convolver4ChanVector Instance = new ();

		private Convolver4ChanVector() { }

		int IConvolver.Channels => (int)channels;
		int IConvolver.MapChannels => (int)channels;

		unsafe void IConvolver.ConvolveSourceLine(byte* istart, byte* tstart, int cb, byte* mapxstart, int smapx, int smapy)
		{
			float* tp = (float*)tstart, tpe = (float*)(tstart + (uint)cb);
			uint* pmapx = (uint*)mapxstart;
			nuint kstride = (nuint)smapx * channels;
			nuint tstride = (nuint)smapy * channels;
			nuint vcnt = kstride / vector4Count;

			float* buff = tp;
			if (VectorF.Count == 8 && vcnt >= 2)
			{
				float* tmp = stackalloc float[VectorF.Count];
				buff = tmp;
			}

			while (tp < tpe)
			{
				nuint ix = *pmapx++;
				nuint lcnt = vcnt;

				float* ip = (float*)istart + ix * channels;
				float* mp = (float*)(mapxstart + *pmapx++);

				Vector4 av0;

				if (VectorF.Count == 8 && lcnt >= 2)
				{
					var ax0 = VectorF.Zero;

					for (; lcnt >= 8; lcnt -= 8)
					{
						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						var iv2 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 2);
						var iv3 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 3);
						ip += VectorF.Count * 4;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						ax0 += iv2 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 2);
						ax0 += iv3 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 3);
						mp += VectorF.Count * 4;
					}

					if (lcnt >= 6)
					{
						lcnt -= 6;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						var iv2 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 2);
						ip += VectorF.Count * 3;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						ax0 += iv2 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 2);
						mp += VectorF.Count * 3;
					}
					else if (lcnt >= 4)
					{
						lcnt -= 4;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						ip += VectorF.Count * 2;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						mp += VectorF.Count * 2;
					}
					else if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						ip += VectorF.Count;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						mp += VectorF.Count;
					}

					Unsafe.WriteUnaligned(buff, ax0);
					av0 = Unsafe.ReadUnaligned<Vector4>(buff) + Unsafe.ReadUnaligned<Vector4>(buff + vector4Count);
				}
				else
				{
					av0 = Vector4.Zero;

					for (; lcnt >= 4; lcnt -= 4)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count);
						var iv2 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count * 2);
						var iv3 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count * 3);
						ip += vector4Count * 4;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						av0 += iv2 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count * 2);
						av0 += iv3 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count * 3);
						mp += vector4Count * 4;
					}

					if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count);
						ip += vector4Count * 2;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						mp += vector4Count * 2;
					}
				}

				if (lcnt != 0)
				{
					var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);

					av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
				}

				Unsafe.WriteUnaligned(tp, av0);
				tp += tstride;
			}
		}

		unsafe void IConvolver.WriteDestLine(byte* tstart, byte* ostart, int ox, int ow, byte* pmapy, int smapy)
		{
			float* op = (float*)ostart;
			nuint tstride = (nuint)smapy * channels;
			nuint vcnt = tstride / vector4Count, nox = (nuint)ox;

			float* buff = op;
			if (VectorF.Count == 8 && vcnt >= 2)
			{
				float* tmp = stackalloc float[VectorF.Count];
				buff = tmp;
			}

			for (nuint xc = nox + (nuint)ow; nox < xc; nox++)
			{
				nuint lcnt = vcnt;

				float* tp = (float*)tstart + nox * tstride;
				float* mp = (float*)pmapy;

				Vector4 av0;

				if (VectorF.Count == 8 && lcnt >= 2)
				{
					var ax0 = VectorF.Zero;

					for (; lcnt >= 4; lcnt -= 4)
					{
						var iv0 = Unsafe.ReadUnaligned<VectorF>(tp);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(tp + VectorF.Count);
						tp += VectorF.Count * 2;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						mp += VectorF.Count * 2;
					}

					if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(tp);
						tp += VectorF.Count;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						mp += VectorF.Count;
					}

					Unsafe.WriteUnaligned(buff, ax0);
					av0 = Unsafe.ReadUnaligned<Vector4>(buff) + Unsafe.ReadUnaligned<Vector4>(buff + vector4Count);
				}
				else
				{
					av0 = Vector4.Zero;

					for (; lcnt >= 2; lcnt -= 2)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(tp);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(tp + vector4Count);
						tp += vector4Count * 2;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						mp += vector4Count * 2;
					}
				}

				if (lcnt != 0)
				{
					var iv0 = Unsafe.ReadUnaligned<Vector4>(tp);

					av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
				}

				Unsafe.WriteUnaligned(op, av0);
				op += channels;
			}
		}

		public override string ToString() => nameof(Convolver4ChanVector);
	}

	internal sealed partial class Convolver3ChanVector : IConvolver
	{
		private const uint channels = 3;
		private const uint vector4Count = 4;

		public static readonly Convolver3ChanVector Instance = new ();

		private Convolver3ChanVector() { }

		int IConvolver.Channels => (int)channels;
		int IConvolver.MapChannels => (int)channels;

		unsafe void IConvolver.ConvolveSourceLine(byte* istart, byte* tstart, int cb, byte* mapxstart, int smapx, int smapy)
		{
			float* tp = (float*)tstart, tpe = (float*)(tstart + (uint)cb);
			uint* pmapx = (uint*)mapxstart;
			nuint kstride = (nuint)smapx * channels;
			nuint tstride = (nuint)smapy * 4;
			nuint vcnt = kstride / vector4Count;

			float* buff = tp;
			if (VectorF.Count == 8 && vcnt >= 12)
			{
				float* tmp = stackalloc float[VectorF.Count];
				buff = tmp;
			}

			while (tp < tpe)
			{
				nuint ix = *pmapx++;
				nuint lcnt = vcnt;

				float* ip = (float*)istart + ix * channels, ipe = ip + kstride;
				float* mp = (float*)(mapxstart + *pmapx++);

				Vector4 av0, av1, av2;

				if (VectorF.Count == 8 && lcnt >= 12)
				{
					var ax0 = VectorF.Zero;
					var ax1 = VectorF.Zero;
					var ax2 = VectorF.Zero;

					for (; lcnt >= 6; lcnt -= 6)
					{
						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						var iv2 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 2);
						ip += VectorF.Count * 3;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax1 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						ax2 += iv2 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 2);
						mp += VectorF.Count * 3;
					}

					Unsafe.WriteUnaligned(buff, ax0);
					av0 = Unsafe.ReadUnaligned<Vector4>(buff);
					av1 = Unsafe.ReadUnaligned<Vector4>(buff + vector4Count);

					Unsafe.WriteUnaligned(buff, ax1);
					av2 = Unsafe.ReadUnaligned<Vector4>(buff);
					av0 += Unsafe.ReadUnaligned<Vector4>(buff + vector4Count);

					Unsafe.WriteUnaligned(buff, ax2);
					av1 += Unsafe.ReadUnaligned<Vector4>(buff);
					av2 += Unsafe.ReadUnaligned<Vector4>(buff + vector4Count);
				}
				else
				{
					av0 = av1 = av2 = Vector4.Zero;
				}

				for (; lcnt >= 3; lcnt -= 3)
				{
					var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
					var iv1 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count);
					var iv2 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count * 2);
					ip += vector4Count * 3;

					av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
					av1 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
					av2 += iv2 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count * 2);
					mp += vector4Count * 3;
				}

				float a0, a1, a2;
				if (vcnt == 0)
				{
					a0 = a1 = a2 = 0f;
				}
				else
				{
					a0 = av0.X + av0.W + av1.Z + av2.Y;
					a1 = av0.Y + av1.X + av1.W + av2.Z;
					a2 = av0.Z + av1.Y + av2.X + av2.W;
				}

				while (ip < ipe)
				{
					a0 += ip[0] * mp[0];
					a1 += ip[1] * mp[1];
					a2 += ip[2] * mp[2];

					ip += channels;
					mp += channels;
				}

				tp[0] = a0;
				tp[1] = a1;
				tp[2] = a2;
				tp += tstride;
			}
		}

		unsafe void IConvolver.WriteDestLine(byte* tstart, byte* ostart, int ox, int ow, byte* pmapy, int smapy) => throw new NotImplementedException();

		public override string ToString() => nameof(Convolver3ChanVector);
	}

	internal sealed partial class Convolver1ChanVector : IConvolver
	{
		private const uint channels = 1;
		private const uint vector4Count = 4;

		public static readonly Convolver1ChanVector Instance = new ();

		private Convolver1ChanVector() { }

		int IConvolver.Channels => (int)channels;
		int IConvolver.MapChannels => (int)channels;

		unsafe void IConvolver.ConvolveSourceLine(byte* istart, byte* tstart, int cb, byte* mapxstart, int smapx, int smapy)
		{
			float* tp = (float*)tstart, tpe = (float*)(tstart + (uint)cb);
			uint* pmapx = (uint*)mapxstart;
			nuint kstride = (nuint)smapx * channels;
			nuint tstride = (nuint)smapy * channels;
			nuint vcnt = kstride / vector4Count;

			var m4 = Vector4.Zero;
			var mF = VectorF.Zero;

			if (VectorF.Count == 8 && vcnt >= 2)
				mF = VectorF.One;
			else
				m4 = Vector4.One;

			while (tp < tpe)
			{
				nuint ix = *pmapx++;
				nuint lcnt = vcnt;

				float* ip = (float*)istart + ix * channels, ipe = ip + kstride;
				float* mp = (float*)(mapxstart + *pmapx++);

				float a0;

				if (VectorF.Count == 8 && lcnt >= 2)
				{
					var ax0 = VectorF.Zero;

					for (; lcnt >= 8; lcnt -= 8)
					{
						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						var iv2 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 2);
						var iv3 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 3);
						ip += VectorF.Count * 4;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						ax0 += iv2 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 2);
						ax0 += iv3 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 3);
						mp += VectorF.Count * 4;
					}

					if (lcnt >= 6)
					{
						lcnt -= 6;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						var iv2 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count * 2);
						ip += VectorF.Count * 3;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						ax0 += iv2 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count * 2);
						mp += VectorF.Count * 3;
					}
					else if (lcnt >= 4)
					{
						lcnt -= 4;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(ip + VectorF.Count);
						ip += VectorF.Count * 2;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						mp += VectorF.Count * 2;
					}
					else if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(ip);
						ip += VectorF.Count;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						mp += VectorF.Count;
					}

					a0 = Vector.Dot(ax0, mF);
				}
				else
				{
					var av0 = Vector4.Zero;

					for (; lcnt >= 4; lcnt -= 4)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count);
						var iv2 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count * 2);
						var iv3 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count * 3);
						ip += vector4Count * 4;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						av0 += iv2 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count * 2);
						av0 += iv3 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count * 3);
						mp += vector4Count * 4;
					}

					if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(ip + vector4Count);
						ip += vector4Count * 2;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						mp += vector4Count * 2;
					}

					if (lcnt != 0)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(ip);
						ip += vector4Count;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						mp += vector4Count;
					}

					a0 = Vector4.Dot(av0, m4);
				}

				while (ip < ipe)
				{
					a0 += ip[0] * mp[0];

					ip += channels;
					mp += channels;
				}

				tp[0] = a0;
				tp += tstride;
			}
		}

		unsafe void IConvolver.WriteDestLine(byte* tstart, byte* ostart, int ox, int ow, byte* pmapy, int smapy)
		{
			float* op = (float*)ostart;
			nuint tstride = (nuint)smapy * channels;
			nuint vcnt = tstride / vector4Count, nox = (nuint)ox;

			var m4 = Vector4.Zero;
			var mF = VectorF.Zero;

			if (VectorF.Count == 8 && vcnt >= 2)
				mF = VectorF.One;
			else
				m4 = Vector4.One;

			for (nuint xc = nox + (nuint)ow; nox < xc; nox++)
			{
				nuint lcnt = vcnt;

				float* tp = (float*)tstart + nox * tstride, tpe = tp + tstride;
				float* mp = (float*)pmapy;

				float a0;

				if (VectorF.Count == 8 && lcnt >= 2)
				{
					var ax0 = VectorF.Zero;

					for (; lcnt >= 4; lcnt -= 4)
					{
						var iv0 = Unsafe.ReadUnaligned<VectorF>(tp);
						var iv1 = Unsafe.ReadUnaligned<VectorF>(tp + VectorF.Count);
						tp += VectorF.Count * 2;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						ax0 += iv1 * Unsafe.ReadUnaligned<VectorF>(mp + VectorF.Count);
						mp += VectorF.Count * 2;
					}

					if (lcnt >= 2)
					{
						lcnt -= 2;

						var iv0 = Unsafe.ReadUnaligned<VectorF>(tp);
						tp += VectorF.Count;

						ax0 += iv0 * Unsafe.ReadUnaligned<VectorF>(mp);
						mp += VectorF.Count;
					}

					a0 = Vector.Dot(ax0, mF);
				}
				else
				{
					var av0 = Vector4.Zero;

					for (; lcnt >= 2; lcnt -= 2)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(tp);
						var iv1 = Unsafe.ReadUnaligned<Vector4>(tp + vector4Count);
						tp += vector4Count * 2;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						av0 += iv1 * Unsafe.ReadUnaligned<Vector4>(mp + vector4Count);
						mp += vector4Count * 2;
					}

					if (lcnt != 0)
					{
						var iv0 = Unsafe.ReadUnaligned<Vector4>(tp);
						tp += vector4Count;

						av0 += iv0 * Unsafe.ReadUnaligned<Vector4>(mp);
						mp += vector4Count;
					}

					a0 = Vector4.Dot(av0, m4);
				}

				while (tp < tpe)
				{
					a0 += tp[0] * mp[0];

					tp += channels;
					mp += channels;
				}

				op[0] = a0;
				op += channels;
			}
		}

		public override string ToString() => nameof(Convolver1ChanVector);
	}
}
