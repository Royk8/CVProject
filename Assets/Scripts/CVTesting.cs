namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using System;

    public class CVTesting : WebCamera
    {
        WebCamTexture img;
        const float downScale = 0.33f;
        const float minimumAreaDiagonal = 25.0f;

        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = Vector2.zero;

        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            Mat image = Unity.TextureToMat(input, TextureParameters);
            Mat downscaled = image.Resize(Size.Zero, downScale, downScale);

            Vector2 sp = ConvertToImageSpace(startPoint, image.Size());
            Vector2 ep = ConvertToImageSpace(endPoint, image.Size());
            Point location = new Point(Math.Min(sp.x, ep.x), Math.Min(sp.y, ep.y));
            Size size = new Size(Math.Abs(ep.x - sp.x), Math.Abs(ep.y - sp.y));
            var areaRect = new OpenCvSharp.Rect(location, size);
            Rect2d obj = Rect2d.Empty;

            output = Unity.MatToTexture(image, output);
            return true;
        }

        Vector2 ConvertToImageSpace(Vector2 coord, Size size)
        {
            var ri = GetComponent<UnityEngine.UI.RawImage>();

            Vector2 output = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ri.rectTransform, coord, null, out output);

            // pivot is in the center of the rectTransform, we need { 0, 0 } origin
            output.x += size.Width / 2;
            output.y += size.Height / 2;

            // now our image might have various transformations of it's own
            if (!TextureParameters.FlipVertically)
                output.y = size.Height - output.y;

            // downscaling
            output.x *= downScale;
            output.y *= downScale;

            return output;
        }

        protected override void Awake()
        {
            base.Awake();
            forceFrontalCamera = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}
