using System.Collections;
using System.Collections.Generic;

namespace CallBackData
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, TT>(T arg1, TT arg2);
    public delegate void Callback<T, TT, TTT>(T arg1, TT arg2, TTT arg3);
    public delegate void Callback<T, TT, TTT, TTTT>(T arg1, TT arg2, TTT arg3,TTTT arg4);
    public delegate void Callback<T, TT, TTT, TTTT,TTTTT>(T arg1, TT arg2, TTT arg3, TTTT arg4, TTTTT arg5);
}