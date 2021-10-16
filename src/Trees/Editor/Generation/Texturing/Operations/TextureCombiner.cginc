#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

sampler2D _RTex;
sampler2D _GTex;
sampler2D _BTex;
sampler2D _ATex;

float4 _RTex_ST;
float4 _GTex_ST;
float4 _BTex_ST;
float4 _ATex_ST;

half _RTexChannel;
half _GTexChannel;
half _BTexChannel;
half _ATexChannel;

fixed _RTexInvert;
fixed _GTexInvert;
fixed _BTexInvert;
fixed _ATexInvert;

fixed _RTexPackStyle;
fixed _GTexPackStyle;
fixed _BTexPackStyle;
fixed _ATexPackStyle;

fixed _RTexFixedValue;
fixed _GTexFixedValue;
fixed _BTexFixedValue;
fixed _ATexFixedValue;

half _RTexMode;
half _GTexMode;
half _BTexMode;
half _ATexMode;

fixed _ColorStyle;
fixed _ColorBoost;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 invert = fixed4(_RTexInvert, _GTexInvert, _BTexInvert, _ATexInvert);
    
    fixed4 r_mask = float4( _RTexChannel >= 0.0 && _RTexChannel < 1.0 ? 1.0 : 0.0,
                            _RTexChannel >= 1.0 && _RTexChannel < 2.0 ? 1.0 : 0.0,
                            _RTexChannel >= 2.0 && _RTexChannel < 3.0 ? 1.0 : 0.0,
                            _RTexChannel >= 3.0 && _RTexChannel < 4.0 ? 1.0 : 0.0);  
    
    fixed4 g_mask = float4( _GTexChannel >= 0.0 && _GTexChannel < 1.0 ? 1.0 : 0.0,
                            _GTexChannel >= 1.0 && _GTexChannel < 2.0 ? 1.0 : 0.0,
                            _GTexChannel >= 2.0 && _GTexChannel < 3.0 ? 1.0 : 0.0,
                            _GTexChannel >= 3.0 && _GTexChannel < 4.0 ? 1.0 : 0.0); 
    
    fixed4 b_mask = float4( _BTexChannel >= 0.0 && _BTexChannel < 1.0 ? 1.0 : 0.0,
                            _BTexChannel >= 1.0 && _BTexChannel < 2.0 ? 1.0 : 0.0,
                            _BTexChannel >= 2.0 && _BTexChannel < 3.0 ? 1.0 : 0.0,
                            _BTexChannel >= 3.0 && _BTexChannel < 4.0 ? 1.0 : 0.0); 
    
    fixed4 a_mask = float4( _ATexChannel >= 0.0 && _ATexChannel < 1.0 ? 1.0 : 0.0,
                            _ATexChannel >= 1.0 && _ATexChannel < 2.0 ? 1.0 : 0.0,
                            _ATexChannel >= 2.0 && _ATexChannel < 3.0 ? 1.0 : 0.0,
                            _ATexChannel >= 3.0 && _ATexChannel < 4.0 ? 1.0 : 0.0);

    fixed4 r_tex = tex2D(_RTex, i.uv);
    fixed4 g_tex = tex2D(_GTex, i.uv);
    fixed4 b_tex = tex2D(_BTex, i.uv);
    fixed4 a_tex = tex2D(_ATex, i.uv);

    
    r_tex = _RTexMode == 0 
        ? r_tex 
        : _RTexMode == 1 
            ? fixed4(LinearToGammaSpace(r_tex), r_tex.a)
            //? r_tex 
            : _RTexMode == 2 
                ? fixed4(UnpackNormalDXT5nm(r_tex) * .5 + .5, 1) : r_tex.xxxx;                
                
    g_tex = _GTexMode == 0 
        ? g_tex 
        : _GTexMode == 1 
            ? fixed4(LinearToGammaSpace(g_tex), g_tex.a)
            //? g_tex 
            : _GTexMode == 2 
                ? fixed4(UnpackNormalDXT5nm(g_tex) * .5 + .5, 1) : g_tex.xxxx;
                
    b_tex = _BTexMode == 0 
        ? b_tex 
        : _BTexMode == 1 
            ? fixed4(LinearToGammaSpace(b_tex), b_tex.a)
            //? b_tex
            : _BTexMode == 2 
                ? fixed4(UnpackNormalDXT5nm(b_tex) * .5 + .5, 1) : b_tex.xxxx;
                
    a_tex = _ATexMode == 0 
        ? a_tex 
        : _ATexMode == 1 
            ? fixed4(LinearToGammaSpace(a_tex), a_tex.a)
            //? a_tex 
            : _ATexMode == 2 
                ? fixed4(UnpackNormalDXT5nm(a_tex) * .5 + .5, 1) : a_tex.xxxx;
    
    
    fixed4 r4 = r_mask * (invert[0] == 1.0 ? (1 - r_tex) : r_tex);
    fixed4 g4 = g_mask * (invert[1] == 1.0 ? (1 - g_tex) : g_tex);
    fixed4 b4 = b_mask * (invert[2] == 1.0 ? (1 - b_tex) : b_tex);
    fixed4 a4 = a_mask * (invert[3] == 1.0 ? (1 - a_tex) : a_tex);
    
    r4 = _RTexPackStyle >= 0.0 && _RTexPackStyle < 1.0 ? r4 :
         _RTexPackStyle >= 1.0 && _RTexPackStyle < 2.0 ? r4*2 : (r4-.5)*2;
    g4 = _GTexPackStyle >= 0.0 && _GTexPackStyle < 1.0 ? g4 :
         _GTexPackStyle >= 1.0 && _GTexPackStyle < 2.0 ? g4*2 : (g4-.5)*2;
    b4 = _BTexPackStyle >= 0.0 && _BTexPackStyle < 1.0 ? b4 :
         _BTexPackStyle >= 1.0 && _BTexPackStyle < 2.0 ? b4*2 : (b4-.5)*2;
    a4 = _ATexPackStyle >= 0.0 && _ATexPackStyle < 1.0 ? a4 :
         _ATexPackStyle >= 1.0 && _ATexPackStyle < 2.0 ? a4*2 : (a4-.5)*2;
    
    fixed r = r4.r + r4.g + r4.b + r4.a;
    fixed g = g4.r + g4.g + g4.b + g4.a;
    fixed b = b4.r + b4.g + b4.b + b4.a;
    fixed a = a4.r + a4.g + a4.b + a4.a;
    
    if (_RTexFixedValue >= 0) r = _RTexFixedValue;
    if (_GTexFixedValue >= 0) g = _GTexFixedValue;
    if (_BTexFixedValue >= 0) b = _BTexFixedValue;
    if (_ATexFixedValue >= 0) a = _ATexFixedValue;
    
    fixed4 output = saturate(fixed4(r,g,b,a));
    
        
    if (_ColorStyle == 1.0)
    {
        output *= _ColorBoost;
        output.a = a;
    }
    else if (_ColorStyle == 2.0)
    {
        fixed luminance = Luminance(output);        
        output = fixed4(luminance, luminance, luminance, a);
    }
    else if (_ColorStyle == 3.0)
    {
        fixed luminance = Luminance(output * _ColorBoost);        
        output = fixed4(luminance, luminance, luminance, a);
    }
    
    output = saturate(output);
    
    
    return output;
}