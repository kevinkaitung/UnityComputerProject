#ifndef EASING_CUBIC
#define EASING_CUBIC

void CubicIn_float(const float In, out float Out)
{
    Out = In * In * In;
}

void CubicOut_float(const float In, out float Out)
{
    const float f = In - 1;
    Out = f * f * f + 1;
}

void CubicInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 4 * In * In * In;
    else
    {
        const float f = 2 * In - 2;
        Out = 0.5 * f * f * f + 1;
    }
}

#endif
