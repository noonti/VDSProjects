#ifndef _SYSTYPES_H_
#define _SYSTYPES_H_

#define LOCAL	static    
#define EXTERN	extern  
#define VOID		void

/* Define standard data types.  These definitions allow Nucleus PLUS to
   perform in the same manner on different target platforms.  */
typedef unsigned long	UNSIGNED;
typedef long		SIGNED;
typedef unsigned char	DATA_ELEMENT;
typedef DATA_ELEMENT	OPTION;
typedef int		STATUS;
typedef unsigned char	UNSIGNED_CHAR;
typedef char		CHAR;
typedef int		INT;
typedef unsigned long *	UNSIGNED_PTR;
typedef unsigned char *	BYTE_PTR;

/* Define the following by byte size */
typedef char		INT8;  
typedef unsigned char	UINT8; 
typedef signed short	INT16; 
typedef unsigned short	UINT16; 
typedef signed long	INT32;  
typedef unsigned long	UINT32;  

typedef char		int8;             
typedef unsigned char	uint8;   
typedef short int	int16;       
typedef unsigned short	uint16;  
typedef long int	int32;         
typedef unsigned int	uint32;   

typedef unsigned char	BOOL; 
typedef unsigned char	U8;
typedef unsigned short	U16;
typedef unsigned long	U32;

typedef signed char 	s8;
typedef unsigned char	u8;
typedef signed short 	s16;
typedef unsigned short	u16;
typedef signed int 	s32;
typedef unsigned int	u32;
typedef signed long long 	s64;
typedef unsigned long long 	u64;

#define BITS_PER_LONG 32

/* Dma addresses are 32-bits wide.  */

typedef u32 dma_addr_t;

typedef unsigned long phys_addr_t;
typedef unsigned long phys_size_t;


typedef unsigned char		uchar;
typedef volatile unsigned long	vu_long;
typedef volatile unsigned short vu_short;
typedef volatile unsigned char	vu_char;

/* bsd */
typedef unsigned char	u_char;
typedef unsigned short	u_short;
typedef unsigned int	u_int;
typedef unsigned long	u_long;

/* sysv */
typedef unsigned char	unchar;
typedef unsigned short	ushort;
typedef unsigned int	uint;
typedef unsigned long	ulong;

typedef BOOL		boolean;

///// modify kwj /////////////////
typedef unsigned char u8_t;
typedef unsigned short u16_t;
typedef unsigned char uint8_t;
////////////////////////////////////////

#if defined(USE_GCC) // by jwank 080101
 #define INLINE inline
 #define __PACKED__	__attribute__ ((packed))
#else // for ADS Compiler
 #define INLINE __inline
 #define __PACKED__	__packed 
#endif

/*----------------*/
/* Boolean values */
/*----------------*/
#define TRUE                1
#define FALSE               0


#define DEBUG_PRINT(s)		0
#define DEBUG_PRINTs(f,s)	0
#define DEBUG_PRINTi(f,i)	0

#define indexof(a) (sizeof(a)/sizeof((a)[0]))
#define MIN(a,b) (a <= b ? a : b)

#define MSB(word)      (u8)(((u16)(word) >> 8) & 0xff)
#define LSB(word)      (u8)((u16)(word) & 0xff)

#define SWAP_ENDIAN(word)   ((u8*)&word)[0] ^= ((u8*)&word)[1];\
                     ((u8*)&word)[1] ^= ((u8*)&word)[0];\
                     ((u8*)&word)[0] ^= ((u8*)&word)[1]

#endif /* _SYSTYPES_H_ */

