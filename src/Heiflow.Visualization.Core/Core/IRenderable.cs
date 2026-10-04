using System;

namespace HUST.WREIS.Dot3D.Renderable
{
	/// <summary>
	/// 
	/// </summary>
	interface IRenderable : IDisposable
	{
		void Initialize(DrawArgs drawArgs);
		void Update(DrawArgs drawArgs);
		void Render(DrawArgs drawArgs);
	}
}
