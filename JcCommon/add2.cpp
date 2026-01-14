#include <iostream>
#include <stdint.h>
#include "strconv2.h"

extern "C" __declspec(dllexport)
int32_t add2(int32_t a, int32_t b)
{
	std::cout << a << "+" << b << std::endl;
	return a + b;
}

extern "C" __declspec(dllexport)
void hello(const char *msg)
{
    //dbgmsg(u8"msg", u8"msg=%s\n", msg);
	formatA(std::cout, u8"msg=%s\n", msg);
}

extern "C" __declspec(dllexport)
const char *greeting(const char *name)
{
    //dbgmsg(u8"name", u8"name=%s\n", name);
	static thread_local std::string result;
	result = format(u8"Helloハロー© %s!", name);
	return result.c_str();
}
