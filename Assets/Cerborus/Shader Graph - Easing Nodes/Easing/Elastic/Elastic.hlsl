#ifndef EASING_ELASTIC
#define EASING_ELASTIC

#include "../EasingConstants.hlsl"

void ElasticIn_float(const float In, out float Out)
{
    Out = sin(13 * E_PI2 * In) * pow(2, 10 * (In - 1));
}

void ElasticOut_float(const float In, out float Out)
{
    Out = sin(-13 * E_PI2 * (In + 1)) * pow(2, -10 * In) + 1;
}

void ElasticInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 0.5 * sin(13 * E_PI2 * (2 * In)) * pow(2, 10 * (2 * In - 1));
    else
        Out = 0.5 * (sin(-13 * E_PI2 * (2 * In - 1 + 1)) * pow(2, -10 * (2 * In - 1)) + 2);
}

#endif
