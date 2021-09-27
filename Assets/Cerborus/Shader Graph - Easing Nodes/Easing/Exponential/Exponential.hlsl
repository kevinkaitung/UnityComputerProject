#ifndef EASING_EXPONENTIAL
#define EASING_EXPONENTIAL

void ExponentialIn_float(const float In, out float Out)
{
    Out = In == 0.0 ? In : pow(2, 10 * (In - 1));
}

void ExponentialOut_float(const float In, out float Out)
{
    Out = In == 1.0 ? In : 1 - pow(2, -10 * In);
}

void ExponentialInOut_float(const float In, out float Out)
{
    if (In == 0.0 || In == 1.0)
        Out = In;
    else if (In < 0.5)
        Out = 0.5 * pow(2, 20 * In - 10);
    else
        Out = -0.5 * pow(2, -20 * In + 10) + 1;
}

#endif
