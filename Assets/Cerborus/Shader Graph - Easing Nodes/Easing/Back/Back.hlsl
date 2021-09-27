#ifndef EASING_BACK
#define EASING_BACK

#include "../EasingConstants.hlsl"

void BackIn_float(const float In, out float Out)
{
    Out = In * In * In - In * sin(In * E_PI);
}

void BackOut_float(const float In, out float Out)
{
    const float f = 1 - In;
    Out = 1 - (f * f * f - f * sin(f * E_PI));
}

void BackInOut_float(const float In, out float Out)
{
    float f;

    if (In < 0.5)
    {
        f = 2 * In;
        Out = 0.5 * (f * f * f - f * sin(f * E_PI));
    }
    else
    {
        f = 1 - (2 * In - 1);
        Out = 0.5 * (1 - (f * f * f - f * sin(f * E_PI))) + 0.5;
    }
}

#endif
