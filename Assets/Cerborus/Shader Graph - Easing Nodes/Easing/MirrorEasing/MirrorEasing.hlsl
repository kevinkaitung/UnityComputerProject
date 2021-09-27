#ifndef MIRROR_EASING
#define MIRROR_EASING

void MirrorEasing_float(float In, out float Out)
{
    if (In > 0.5)
        In = 1 - In;

    Out = In * 2;
}

#endif
