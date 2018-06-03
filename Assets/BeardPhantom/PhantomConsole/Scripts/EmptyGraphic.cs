using UnityEngine.UI;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Used as a simple UI hitbox
    /// </summary>
    public class EmptyGraphic : Graphic
    {
        /// <inheritdoc />
        public override void SetMaterialDirty() { }

        /// <inheritdoc />
        public override void SetVerticesDirty() { }

        /// <inheritdoc />
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // Ignore mesh building
            vh.Clear();
        }
    }
}