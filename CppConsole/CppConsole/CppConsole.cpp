// CppConsole.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"


int _tmain(int argc, _TCHAR* argv[])
{
    printf("test %d\n", 777);

    int n;
    int *p;

    n = 151;
    p = &n;// & tells address of an object
    
    printf("n : %d\n", n);
    printf("n : %d\n", *p);// * gives an object at following address
    
    printf("n address : $%x\n", &n);
    printf("p value : $%x\n", p);
    printf("p is located : $%x\n", &p);

    return 0;
}

