#ifndef EASING_CIRCULAR
#define EASING_CIRCULAR

void CircularIn_float(const float In, out float Out)
{
    Out = 1 - sqrt(1 - In * In);
}

void CircularOut_float(const float In, out float Out)
{
    Out = sqrt((2 - In) * In);
}

void CircularInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 0.5 * (1 - sqrt(1 - 4 * (In * In)));
    else
        Out = 0.5 * (sqrt(-(2 * In - 3) * (2 * In - 1)) + 1);
}

#endif
