﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Renderer
{
    internal class CameraViewRenderer : ViewRenderer<CameraView, UICameraPreview>
    {
        UICameraPreview liveCameraStream;
        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                if (liveCameraStream != null)
                {
                    liveCameraStream.OnDetected -= OnDetected;
                }

                return;
            }

            if (e.NewElement != null && Control == null)
            {
                var cameraView = e.NewElement;
                liveCameraStream = new UICameraPreview(this, cameraView.CameraFacing, cameraView.CaptureQuality);
                SetNativeControl(liveCameraStream);
                liveCameraStream.OnDetected += OnDetected;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CameraView.TorchOnProperty.PropertyName)
            {
                HandleTorch();
            }
            else if (e.PropertyName == CameraView.CameraFacingProperty.PropertyName)
            {
                liveCameraStream.ChangeCamera(Element.CameraFacing);
            }
            else if (e.PropertyName == CameraView.CaptureQualityProperty.PropertyName)
            {
                liveCameraStream.ChangeSessionPreset(Element.CaptureQuality);
            }
        }

        private void HandleTorch()
        {
            if (Element == null || liveCameraStream == null) return;
            if (Element.TorchOn && liveCameraStream.IsTorchOn() || !Element.TorchOn && !liveCameraStream.IsTorchOn())
                return;
            liveCameraStream.ToggleFlashlight();
        }


        private void OnDetected(object sender, List<BarcodeResult> arg) =>
            Element?.TriggerOnDetected(arg);


    }
}
