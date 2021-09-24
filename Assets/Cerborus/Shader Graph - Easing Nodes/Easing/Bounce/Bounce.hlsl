#ifndef EASING_BOUNCE
#define EASING_BOUNCE

static const float a = 4 / 11.0;

static const float b = 8 / 11.0;
static const float b1 = 363 / 40.0;
static const float b2 = 99 / 10.0;
static const float b3 = 17 / 5.0;

static const float c = 9 / 10.0;
static const float c1 = 4356 / 361.0;
static const float c2 = 35442 / 1805.0;
static const float c3 = 16061 / 1805.0;

static const float d1 = 54 / 5.0;
static const float d2 = 513 / 25.0;
static const float d3 = 268 / 25.0;

float BounceOutFunc_float(const float In)
{
    if (In < a) return 121 * In * In / 16.0;
    if (In < b) return b1 * In * In - b2 * In + b3;
    if (In < c) return c1 * In * In - c2 * In + c3;
    return d1 * In * In - d2 * In + d3;
}

void BounceIn_float(const float In, out float Out)
{
    Out = 1 - BounceOutFunc_float(1 - In);
}

void BounceOut_float(const float In, out float Out)
{
    Out = BounceOutFunc_float(In);
}

void BounceInOut_float(const float In, out float Out)
{
    if (In < 0.5)
        Out = 0.5 * (1 - BounceOutFunc_float(1 - In * 2));
    else
        Out = 0.5 * BounceOutFunc_float(In * 2 - 1) + 0.5;
}

#endif
