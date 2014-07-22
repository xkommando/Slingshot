Slingshot
=======

_A simple functional language implemented in C#_


- no keywords, only functions: built-in functions and Slingshot functions.
  'if else', 'while', 'hash' are built-in funcions, 'map' 'reduce' are Slingshot functions 

- every thing is a expression // except for comment

- value is immutable:   (def a "behold a string of Slingshot")

- the value of {...} is the value of the last expression within {} : 

(def a {(def b 2) (def c 3) (def d 4)}) // a is SSInteger 4

- this is a list:   [1,   '2',   "3456789",   False,   ["new list" 'a' 5]   ]

- functions can be defined anywhere: (def   f-1(  { (def   f-2()) } )  )

Code snippet from the Slingshot Standard Library

``` Lisp
(def map  
	(func (f alist)
		(if (null? alist)
			alist
			(cons (f (car alist))  (map f (cdr alist)) )
        )))

(def reduce 
	(func (init op ls)
		(if (null? ls)
			init
			(op (car ls) (reduce init op (cdr ls)) )
		)))
```
Algorithem Library example
``` Lisp
(def qsort
	(func(ls compare)
		(if (or (null? ls) (== (length ls) 1) )
			ls
			{ (def _pivot (car ls) )
				(def _left (filter  (func(a)(compare a _pivot))  ls ) )
				(def _right (filter  (func(a)(compare _pivot a)) ls ) )
				(append (append (qsort _left compare) _pivot)
							(qsort _right compare) )
		})))

// return integer -1 if search failed 
(require elem-at)
(def bin-search
	(func(ls val)
		(if(null? ls)
			-1
			{(def _f(func(_min_idx _max_idx)
				(if (< (- _max_idx _min_idx) 2)
					-1
					{(def _m (to-int (/ (+ _max_idx _min_idx) 2)))
						(if (< (elem-at ls _m) val)
							(_f _m _max_idx)
						elif (< val (elem-at ls _m))
								(_f _min_idx _m)
						_m
						)})// if 
					)// functions
				)// def
				(_f 0 (length ls))
			})))
```
Mathematics and Statistics Library example
```Lisp
// test prime numbers recursively
(require while set! even? integer?)
(def r-prime?
	(func(a)
		(if (or (not (integer? a)) 
				(< a 2)
				(even? a)
			) False
			{(def _f (func(b)
						(if (< a (* b b))
							True
						elif  (== 0 (% a b))
								False
							(_f (+ b 2))
						)))
				(_f 3)
			})))

// test prime numbers iteratively
(def prime? 
	(func(a)
		(if (or (not (integer? a)) 
				(< a 2) 
				(even? a) 
			) False
			{ 	(def d 3)
				(def ret True)
				{	(while (< (* d d) a)
						(if (eq? 0 (% a d))
							{ (set! ret False)
								break
							}
							(set! d (+ d 2))
						))
				ret}}
		)))

(def std-deviation
	(func(ls)
		(if(null? ls)
			0
			{ (def _avg (avg ls))
				(def _len (length ls))
				(def _prod (sum (map (func(a)(** a 2)) ls) ) )
				(**	
					(- (/ _prod _len)
						(** _avg 2) )
				0.5)
			})))
```
*in memory of an old friend in my childhood*
