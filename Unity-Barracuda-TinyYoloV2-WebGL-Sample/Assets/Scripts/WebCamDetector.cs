using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using UnityEngine.Profiling;

public class WebCamDetector : MonoBehaviour
{
    [Tooltip("File of YOLO model. If you want to use another than YOLOv2 tiny, it may be necessary to change some const values in YOLOHandler.cs")]
    public NNModel modelFile;

    [Tooltip("RawImage component which will be used to draw resuls.")]
    public RawImage imageRenderer;

    [Range(0.05f, 1f)]
    [Tooltip("The minimum value of box confidence below which boxes won't be drawn.")]
    public float MinBoxConfidence = 0.3f;

    NNHandler nn;
    YOLOHandler yolo;

    WebCamTexture camTexture;
    Texture2D displayingTex;

    TextureScaler textureScaler;

    Color[] colorArray;
    
	Color[] drawBuffer;
    
    public Text text;
    private readonly FPSCounter fpsCounter = new FPSCounter();

    void Start()
    {
        var dev = SelectCameraDevice();
        camTexture = new WebCamTexture(dev);
        camTexture.Play();

        nn = new NNHandler(modelFile);
        yolo = new YOLOHandler(nn);

        textureScaler = new TextureScaler(nn.model.inputs[0].shape[1], nn.model.inputs[0].shape[2]);

        InitializeColorArray();
        StartCoroutine(WebCamTextureInitialize());
    }

    IEnumerator WebCamTextureInitialize()
    {
        while (true) {
            if (camTexture.width > 16 && camTexture.height > 16) {
                break;
            }
            yield return null;
        }
    }

    void Update()
    {
        fpsCounter.Update();

        CaptureAndPrepareTexture(camTexture, ref displayingTex);

        var boxes = yolo.Run(displayingTex);

        DrawBoundingBox(boxes);
        imageRenderer.texture = displayingTex;
        
        string resultText = "";
        resultText = "FPS:" + fpsCounter.FPS.ToString("F2");
        text.text = resultText;
    }

    private void DrawBoundingBox(IEnumerable<YOLOHandler.ResultBox> boxes)
    {
        Color[] pixels = displayingTex.GetPixels();
		drawBuffer = new Color[pixels.Length];
		pixels.CopyTo(drawBuffer, 0);
        
        foreach (var box in boxes) {
            if (box.classes[box.bestClassIdx] > MinBoxConfidence) {
                int x1 = (int)(box.rect.x * displayingTex.width);
                int y1 = (int)(box.rect.y * displayingTex.height);
                y1 = displayingTex.height - y1;
                int x2 = (int)(x1 + (box.rect.width * displayingTex.width));
                int y2 = (int)(y1 + (box.rect.height * displayingTex.height));
                y2 = displayingTex.height - y2;

                var drawPoint1 = new Vector2(x1, y1);
                var drawPoint2 = new Vector2(x2, y1);
                DrawLine(drawPoint1, drawPoint2, colorArray[box.bestClassIdx]);
                drawPoint1 = new Vector2(x2, y1);
                drawPoint2 = new Vector2(x2, y2);
                DrawLine(drawPoint1, drawPoint2, colorArray[box.bestClassIdx]);
                drawPoint1 = new Vector2(x2, y2);
                drawPoint2 = new Vector2(x1, y2);
                DrawLine(drawPoint1, drawPoint2, colorArray[box.bestClassIdx]);
                drawPoint1 = new Vector2(x1, y2);
                drawPoint2 = new Vector2(x1, y1);
                DrawLine(drawPoint1, drawPoint2, colorArray[box.bestClassIdx]);
            }
        }
        displayingTex.SetPixels(drawBuffer);
        displayingTex.Apply();
    }

    private void DrawPoint(Vector2 point, Color color, double brushSize = 2.0f)
    {
        point.x = (int)point.x;
        point.y = (int)point.y;

        int start_x = Mathf.Max(0, (int)(point.x - (brushSize - 1)));
        int end_x = Mathf.Min(displayingTex.width, (int)(point.x + (brushSize + 1)));
        int start_y =  Mathf.Max(0, (int)(point.y - (brushSize - 1)));
        int end_y = Mathf.Min(displayingTex.height, (int)(point.y + (brushSize + 1)));

        for (int x = start_x; x < end_x; x++) {
            for (int y = start_y; y < end_y; y++) {
                double length = Mathf.Sqrt(Mathf.Pow(point.x - x, 2) + Mathf.Pow(point.y - y, 2));
                if (length < brushSize) {
                    drawBuffer.SetValue(color, x + displayingTex.width * y);
                }
            }
        }
    }

    private void DrawLine(Vector2 point1, Vector2 point2, Color color, int lerpNum = 400)
    {
        for(int i=0; i < lerpNum + 1; i++) {
            var point = Vector2.Lerp(point1, point2, i * (1.0f / lerpNum));
            DrawPoint(point, color);
        }
    }

    private void OnDestroy()
    {
        nn.Dispose();
        yolo.Dispose();
        textureScaler.Dispose();

        camTexture.Stop();
    }

    private void CaptureAndPrepareTexture(WebCamTexture camTexture, ref Texture2D tex)
    {
        Profiler.BeginSample("Texture processing");
        TextureCropTools.CropToSquare(camTexture, ref tex);
        textureScaler.Scale(tex);
        Profiler.EndSample();
    }

    /// <summary>
    /// Return first backfaced camera name if avaible, otherwise first possible
    /// </summary>
    string SelectCameraDevice()
    {
        if (WebCamTexture.devices.Length == 0)
            throw new Exception("Any camera isn't avaible!");

        foreach (var cam in WebCamTexture.devices)
        {
            if (!cam.isFrontFacing)
                return cam.name;
        }
        return WebCamTexture.devices[0].name;
    }

    private void InitializeColorArray()
    {
        int color_r, color_g, color_b;
        color_r = 162;
        color_g = 155;
        color_b = 254;
        Color color01 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 129;
        color_g = 236;
        color_b = 236;
        Color color02 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 116;
        color_g = 185;
        color_b = 255;
        Color color03 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 253;
        color_g = 121;
        color_b = 168;
        Color color04 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 223;
        color_g = 230;
        color_b = 233;
        Color color05 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 0;
        color_g = 184;
        color_b = 148;
        Color color06 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 0;
        color_g = 206;
        color_b = 201;
        Color color07 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 9;
        color_g = 132;
        color_b = 227;
        Color color08 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 108;
        color_g = 92;
        color_b = 231;
        Color color09 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 178;
        color_g = 190;
        color_b = 195;
        Color color10 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 255;
        color_g = 234;
        color_b = 167;
        Color color11 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 250;
        color_g = 177;
        color_b = 160;
        Color color12 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 255;
        color_g = 118;
        color_b = 117;
        Color color13 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 99;
        color_g = 110;
        color_b = 114;
        Color color14 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 85;
        color_g = 239;
        color_b = 196;
        Color color15 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 253;
        color_g = 203;
        color_b = 110;
        Color color16 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 225;
        color_g = 112;
        color_b = 85;
        Color color17 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 214;
        color_g = 48;
        color_b = 49;
        Color color18 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 232;
        color_g = 67;
        color_b = 147;
        Color color19 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        color_r = 45;
        color_g = 52;
        color_b = 54;
        Color color20 = new Color((float)(color_r/255f), (float)(color_g/255f), (float)(color_b/255f));
        colorArray = new Color[] { color01, color02, color03, color04, color05, color06, 
                                            color07, color08, color09, color10, color11, color12, 
                                            color13, color14, color15, color16, color17, color18, color19, color20 };
    }
}
