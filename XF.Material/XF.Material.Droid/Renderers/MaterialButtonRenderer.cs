﻿using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.Material.Droid.Renderers;
using XF.Material.Forms.UI;
using static XF.Material.Forms.UI.MaterialButton;
using Button = Xamarin.Forms.Button;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]

namespace XF.Material.Droid.Renderers
{
    public sealed class MaterialButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {
        private Color _borderColor;
        private int _borderWidth;
        private float _cornerRadius;
        private Color _disabledBorderColor;
        private Color _disabledColor;
        private MaterialButton _materialButton;
        private Color _normalColor;
        private Color _pressedColor;
        private bool _withIcon;

        public MaterialButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e?.NewElement != null)
            {
                _materialButton = this.Element as MaterialButton;
                _cornerRadius = MaterialHelper.ConvertToDp(_materialButton.CornerRadius);
                _borderWidth = (int)MaterialHelper.ConvertToDp((int)_materialButton.BorderWidth);

                this.Control.SetMinimumWidth((int)MaterialHelper.ConvertToDp(64));
                this.Control.SetAllCaps(_materialButton.AllCaps);
                this.SetupColors();
                this.SetButtonIcon();
                this.UpdateDrawable();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e?.PropertyName == MaterialButtonColorChanged || e?.PropertyName == nameof(MaterialButton.ButtonType))
            {
                this.SetupColors();
                this.UpdateDrawable();
            }
            else if (e?.PropertyName == nameof(MaterialButton.Image))
            {
                this.SetButtonIcon();
            }
            else if (e?.PropertyName == nameof(MaterialButton.AllCaps))
            {
                this.Control.SetAllCaps(_materialButton.AllCaps);
            }
            else if (e?.PropertyName == nameof(Button.TextColor))
            {
                this.SetTextColors();
            }
        }

        private void CreateContainedButtonDrawable(bool elevated)
        {
            var normalStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, _normalColor, _borderColor);
            var disabledStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, _disabledColor, _disabledBorderColor);

            if (Material.IsLollipop)
            {
                var rippleColor = _normalColor.IsColorDark() ? Color.ParseColor("#52FFFFFF") : Color.ParseColor("#52000000");
                var rippleDrawable = _withIcon ? MaterialHelper.GetDrawableCopyFromResource<RippleDrawable>(Resource.Drawable.drawable_ripple_with_icon) : MaterialHelper.GetDrawableCopyFromResource<RippleDrawable>(Resource.Drawable.drawable_ripple);
                var insetDrawable = rippleDrawable.FindDrawableByLayerId(Resource.Id.inset_drawable) as InsetDrawable;
                var statelistDrawable = insetDrawable.Drawable as StateListDrawable;
                this.SetStates(statelistDrawable, normalStateDrawable, normalStateDrawable, disabledStateDrawable);

                rippleDrawable.SetColor(new ColorStateList(new int[][]
                {
                    new int[]{}
                },
                new int[]
                {
                    rippleColor
                }));
                
                this.Control.Background = rippleDrawable;
                this.Control.StateListAnimator = elevated ? AnimatorInflater.LoadStateListAnimator(this.Context, Resource.Animator.material_button_state_list_anim) : null;
            }
            else
            {
                this.CreatePrelollipopDrawable(normalStateDrawable, disabledStateDrawable);
            }
        }

        private void CreateOutlinedButtonDrawable()
        {
            var normalStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, Color.Transparent, _borderColor);
            var disabledStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, Color.Transparent, _disabledBorderColor);

            if (Material.IsLollipop)
            {
                var rippleDrawable = _withIcon ? MaterialHelper.GetDrawableCopyFromResource<RippleDrawable>(Resource.Drawable.drawable_ripple_outlined_with_icon) : MaterialHelper.GetDrawableCopyFromResource<RippleDrawable>(Resource.Drawable.drawable_ripple_outlined);
                var maskDrawable = rippleDrawable.FindDrawableByLayerId(Android.Resource.Id.Mask) as InsetDrawable;
                var rippleMaskGradientDrawable = maskDrawable.Drawable as GradientDrawable;
                rippleMaskGradientDrawable.SetCornerRadius(_cornerRadius);
                var insetDrawable = rippleDrawable.FindDrawableByLayerId(Resource.Id.inset_drawable) as InsetDrawable;
                var statelistDrawable = insetDrawable.Drawable as StateListDrawable;
                this.SetStates(statelistDrawable, normalStateDrawable, normalStateDrawable, disabledStateDrawable);

                
                this.Control.Background = rippleDrawable;
                this.Control.StateListAnimator = null;
            }
            else
            {
                this.CreatePrelollipopDrawable(normalStateDrawable, disabledStateDrawable);
            }
        }

        private void CreatePrelollipopDrawable(Drawable normalStateDrawable, Drawable disabledStateDrawable)
        {
            if (Material.IsJellyBean)
            {
                var stateListDrawable = new StateListDrawable();
                var pressedStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, _pressedColor, _borderColor);
                this.SetStates(stateListDrawable, normalStateDrawable, pressedStateDrawable, disabledStateDrawable);
                
                this.Control.Background = stateListDrawable;
            }
            else
            {
                var insetDrawable = MaterialHelper.GetDrawableCopyFromResource<InsetDrawable>(Resource.Drawable.drawable_selector);
                var stateListDrawable = insetDrawable.Drawable as StateListDrawable;
                var pressedStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, _pressedColor, _borderColor);
                this.SetStates(stateListDrawable, normalStateDrawable, pressedStateDrawable, disabledStateDrawable);
                
                this.Control.Background = insetDrawable;
            }
        }

        private GradientDrawable CreateShapeDrawable(float cornerRadius, int borderWidth, Color backgroundColor, Color borderColor)
        {
            GradientDrawable shapeDrawable = null;

            if (_materialButton.ButtonType != MaterialButtonType.Text)
            {
                shapeDrawable = _withIcon ? MaterialHelper.GetDrawableCopyFromResource<GradientDrawable>(Resource.Drawable.drawable_shape_with_icon) : MaterialHelper.GetDrawableCopyFromResource<GradientDrawable>(Resource.Drawable.drawable_shape);
            }
            else
            {
                shapeDrawable = MaterialHelper.GetDrawableCopyFromResource<GradientDrawable>(Resource.Drawable.drawable_shape_text);
            }

            shapeDrawable.SetCornerRadius(cornerRadius);
            shapeDrawable.SetColor(backgroundColor);
            shapeDrawable.SetStroke(borderWidth, borderColor);

            return shapeDrawable;
        }

        private void CreateTextButtonDrawable()
        {
            var normalStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, Color.Transparent, Color.Transparent);
            var pressedStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, Color.ParseColor("#52000000"), Color.Transparent);
            var disabledStateDrawable = this.CreateShapeDrawable(_cornerRadius, _borderWidth, Color.Transparent, Color.Transparent);
            disabledStateDrawable.SetColor(Color.Transparent);

            if (Material.IsLollipop)
            {
                var rippleDrawable = MaterialHelper.GetDrawableCopyFromResource<RippleDrawable>(Resource.Drawable.drawable_ripple_text);
                var maskDrawable = rippleDrawable.FindDrawableByLayerId(Android.Resource.Id.Mask) as InsetDrawable;
                var rippleMaskGradientDrawable = maskDrawable.Drawable as GradientDrawable;
                rippleMaskGradientDrawable.SetCornerRadius(_cornerRadius);

                
                this.Control.Background = rippleDrawable;
                this.Control.StateListAnimator = null;
            }
            else if (Material.IsJellyBean)
            {
                var stateListDrawable = new StateListDrawable();
                this.SetStates(stateListDrawable, normalStateDrawable, pressedStateDrawable, normalStateDrawable);
                
                this.Control.Background = stateListDrawable;
            }
            else
            {
                var insetDrawable = MaterialHelper.GetDrawableCopyFromResource<InsetDrawable>(Resource.Drawable.drawable_selector);
                var stateListDrawable = insetDrawable.Drawable as StateListDrawable;
                this.SetStates(stateListDrawable, normalStateDrawable, pressedStateDrawable, normalStateDrawable);
                
                this.Control.Background = insetDrawable;
            }
        }

        private void SetButtonIcon()
        {
            _withIcon = !string.IsNullOrEmpty(_materialButton.Image);

            if (_withIcon)
            {
                var fileName = _materialButton.Image.File.Split('.').First();
                var id = this.Resources.GetIdentifier(fileName, "drawable", Material.Context.PackageName);
                var width = (int)MaterialHelper.ConvertToDp(18 + 4);
                var height = (int)MaterialHelper.ConvertToDp(18);
                var left = (int)MaterialHelper.ConvertToDp(4);
                var drawable = MaterialHelper.GetDrawableCopyFromResource<Drawable>(id);
                drawable.SetBounds(left, 0, width, height);
                drawable.SetTint(_materialButton.TextColor.ToAndroid());

                var currentDrawables = this.Control.GetCompoundDrawables();

                if (currentDrawables != null)
                {
                    for (int i = 0; i < currentDrawables.Length && currentDrawables[i] != null; i++)
                    {
                        currentDrawables[i].Dispose();
                    }
                }

                this.Control.SetCompoundDrawables(drawable, null, null, null);
                this.Control.CompoundDrawablePadding = 0;
            }
        }

        private void SetStates(StateListDrawable stateListDrawable, Drawable normalDrawable, Drawable pressedDrawable, Drawable disabledDrawable)
        {
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StatePressed }, pressedDrawable);
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StateFocused, Android.Resource.Attribute.StateEnabled }, pressedDrawable);
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StateEnabled }, normalDrawable);
            stateListDrawable.AddState(new int[] { Android.Resource.Attribute.StateFocused }, pressedDrawable);
            stateListDrawable.AddState(new int[] { }, disabledDrawable);
        }

        private void SetTextColors()
        {
            var states = new int[][]
            {
                new int[] { Android.Resource.Attribute.StatePressed },
                new int[] { Android.Resource.Attribute.StateFocused, Android.Resource.Attribute.StateEnabled },
                new int[] { Android.Resource.Attribute.StateEnabled },
                new int[] { Android.Resource.Attribute.StateFocused },
                new int[] { }
            };

            var colors = new int[]
            {
                _materialButton.TextColor.ToAndroid(),
                _materialButton.TextColor.ToAndroid(),
                _materialButton.TextColor.ToAndroid(),
                _materialButton.TextColor.ToAndroid(),
                _materialButton.TextColor.MultiplyAlpha(0.38).ToAndroid()
             };

            this.Control.SetTextColor(new ColorStateList(states, colors));
        }

        private void SetupColors()
        {
            _normalColor = _materialButton.BackgroundColor.ToAndroid();
            _pressedColor = new Color(_normalColor.IsColorDark() ? (_normalColor + Color.ParseColor("#52FFFFFF")) : (_normalColor + Color.ParseColor("#52000000")));
            _disabledColor = new Color(ColorUtils.SetAlphaComponent(_normalColor, 80));
            _borderColor = _materialButton.BorderColor.ToAndroid();
            _disabledBorderColor = new Color(ColorUtils.SetAlphaComponent(_borderColor, 80));
        }

        private void UpdateDrawable()
        {
            switch (_materialButton.ButtonType)
            {
                case MaterialButtonType.Elevated:
                    this.CreateContainedButtonDrawable(true);
                    break;

                case MaterialButtonType.Flat:
                    this.CreateContainedButtonDrawable(false);
                    break;

                case MaterialButtonType.Text:
                    this.CreateTextButtonDrawable();
                    break;

                case MaterialButtonType.Outlined:
                    this.CreateOutlinedButtonDrawable();
                    break;
            }

            this.SetTextColors();
        }
    }
}