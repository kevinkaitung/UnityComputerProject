#ifndef EASING_STEPS
#define EASING_STEPS

void Steps_float(const float In, float Steps, out float Out)
{
    Steps = max(Steps, 1);
    Out = floor(In * Steps) / Steps;
}

#endif
