#ifndef EASING_QUINTIC
#define EASING_QUINTIC

void QuinticIn_float(const float In, out float Out)
{
    Out = In * In * In * In * In;
}

void QuinticOut_float(const float In, out float Out)
{
    const float f = In - 1;
    Out = f * f * f * f * f + 1;
}

void QuinticInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 16 * In * In * In * In * In;
    else
    {
        const float f = 2 * In - 2;
        Out = 0.5 * f * f * f * f * f + 1;
    }
}

#endif
