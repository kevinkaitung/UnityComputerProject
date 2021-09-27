#ifndef EASING_SIN
#define EASING_SIN

#include "../EasingConstants.hlsl"

void SinIn_float(const float In, out float Out)
{
    Out = sin((In - 1) * E_PI2) + 1;
}

void SinOut_float(const float In, out float Out)
{
    Out = sin(In * E_PI2);
}

void SinInOut_float(const float In, out float Out)
{
    Out = 0.5 * (1 - cos(In * E_PI));
}

#endif
