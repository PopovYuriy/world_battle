using Plugins.ProceduralUIImage.Scripts.Attributes;
using UnityEngine;

namespace Plugins.ProceduralUIImage.Scripts.Modifiers
{
	[ModifierID("Free")]
	public class FreeModifier : ProceduralImageModifier {
		[SerializeField]private Vector4 radius;

		public Vector4 Radius {
			get {
				return radius;
			}
			set {
				radius = value;
			}
		}

		#region implemented abstract members of ProceduralImageModifier

		public override Vector4 CalculateRadius (Rect imageRect){
			return radius;
		}

		#endregion

		protected void OnValidate()
		{
			radius.x = Mathf.Max(0, radius.x);
			radius.y = Mathf.Max(0, radius.y);
			radius.z = Mathf.Max(0, radius.z);
			radius.w = Mathf.Max(0, radius.w);
		}
	}
}
