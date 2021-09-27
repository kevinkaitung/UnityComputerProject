#ifndef EASING_QUADRATIC
#define EASING_QUADRATIC

void QuadraticIn_float(const float In, out float Out)
{
    Out = In * In;
}

void QuadraticOut_float(const float In, out float Out)
{
    Out = -(In * (In - 2));
}

void QuadraticInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 2 * In * In;
    else
        Out = -2 * In * In + 4 * In - 1;
}

#endif
