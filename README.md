Slingshot
=======

_A simple functional language and its C# interpretor_


Example From the Standard Library

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
Algorithem Library
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
(def bin-search
	(func(ls val)
		(if(null? ls)
			-1
			{(def _f(func(_min_idx _max_idx)
					(if (< (- _max_idx _min_idx) 2)
						-1
						{(def _m (to-int (/ (+ _max_idx _min_idx) 2)))
							//(debug _m (elem-at ls _m))
							(if (< (elem-at ls _m) val)
								(_f _m _max_idx)
								(if (< val (elem-at ls _m))
									(_f _min_idx _m)
									_m
								))
						}))
				)
				(_f 0 (length ls))
			})))
```
Mathematics and Statistics Library
```Lisp
(def prime? 
	(func(a)
		(if (or (not (integer? a)) 
				(< a 1) 
				(even? a) 
			) False
			{ 	(def d 3)
				(def ret True)
				{	(while (< (* d d) a)
						(if (eq? 0 (% a d))
							{ (debug d)
								(set! ret False)
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
