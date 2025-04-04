using Plugins.ProceduralUIImage.Scripts.Attributes;
using UnityEngine;

namespace Plugins.ProceduralUIImage.Scripts.Modifiers
{
	[ModifierID("Uniform")]
	public class UniformModifier : ProceduralImageModifier {
		[SerializeField]private float radius;

		public float Radius {
			get {
				return radius;
			}
			set {
				radius = value;
			}
		}

		#region implemented abstract members of ProceduralImageModifier

		public override Vector4 CalculateRadius (Rect imageRect){
			float r = this.radius;
			return new Vector4(r,r,r,r);
		}

		#endregion
	
	}
}
