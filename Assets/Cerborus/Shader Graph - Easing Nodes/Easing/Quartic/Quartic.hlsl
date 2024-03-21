#ifndef EASING_QUARTIC
#define EASING_QUARTIC

void QuarticIn_float(const float In, out float Out)
{
    Out = In * In * In * In;
}

void QuarticOut_float(const float In, out float Out)
{
    const float f = In - 1;
    Out = f * f * f * (1 - In) + 1;
}

void QuarticInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 8 * In * In * In * In;
    else
    {
        const float f = In - 1;
        Out = -8 * f * f * f * f + 1;
    }
}

#endif
