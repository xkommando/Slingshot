// 				Slingshot Standard Library
//					Algorithm Library
// 					version 0.3
// 				Copyright 2014  Cai Bowen


(def zip
	(func(f l1 l2)
		(if (or (null? l1) (null? l2))
			l1
			(cons (f (car l1) (car l2))
				(zip f (cdr l1) (cdr l2))
			)
		)
	)
)


(def filter
	(func (pred? ls)
		(if (null? ls)
			ls
			(if (pred? (car ls))
				(cons (car ls) (filter pred? (cdr ls)))
				(filter pred? (cdr ls))
			)
		)
	)
)

(def map  
	(func (f alist)
		(if (null? alist)
			alist
			(cons (f (car alist))  (map f (cdr alist)) )
        )
	)
)

(def reduce 
	(func (init op ls)
		(if (null? ls)
			init
			(op (car ls) (reduce init op (cdr ls))
			)
		)
	)
)

(def reverse 
	(func(ls)
		(if (null? ls)
			ls
			(append (reverse (cdr ls)) (car ls))
		)
	)
)

// asc order

(require filter append)
(def q-sort
	(func(ls)
		(if (or (null? ls) (== (length ls) 1))
			ls
			{ (def _pivot (car ls) )
				(def _left (filter  (func(a)(< a _pivot))  ls ) )
				(def _right (filter  (func(a)(< _pivot a)) ls ) )
				(append (append (q-sort _left) _pivot)
							(q-sort _right)
				)
			}
		)
	)
)


(require filter append)
(def qsort
	(func(ls compare)
		(if (or (null? ls) (== (length ls) 1) )
			ls
			{ (def _pivot (car ls) )
				(def _left (filter  (func(a)(compare a _pivot))  ls ) )
				(def _right (filter  (func(a)(compare _pivot a)) ls ) )
				(append (append (qsort _left compare) _pivot)
							(qsort _right compare)
				)
			}
		)
	)
)

(require elem-at)
// return integer -1 if search failed 
(def bin-search
	(func(ls val)
		(if(null? ls)
			-1
			{
				(def _f(func(_min_idx _max_idx)
					(if (< (- _max_idx _min_idx) 2)
						-1
						{
							(def _m (to-int (/ (+ _max_idx _min_idx) 2)))
							//(debug _m (elem-at ls _m))
							(if (< (elem-at ls _m) val)
								(_f _m _max_idx)
								(if (< val (elem-at ls _m))
									(_f _min_idx _m)
									_m
									)
								)
						}
						)
					)
				)
				(_f 0 (length ls))
			}
		)
	)
)


//-------------------------------------------------------------------------------------------------
// predefined vars for testing

(def lslen (func(ls)(length ls)))
(def randls (func(ls)(rand)))
(def ls-gen(func(ls)(length ls)))

(def p-ls1 [ 1 2 3 5 9])
(def p-ls2(reverse p-ls1))// 9 5 3 2 1
(def p-str "str of length 16")
(def p-5f 5.0)
(def p-5i 5)

(def p-choose 
	(func(a)
		(switch a
			case 1 1111
			case 2 2222
			case 3 3333
			case 4 4444
			default 9999
		)
	)
)

(def tryc
	(func()(
			try{
				(error "aaaaa")
				1
			}catch(ex){
				(log ex)
				2
			}
		)
	)
)

(reduce 0 (func(a b)(+ a b))

	(filter 
		(func(a)(> a 20))

	(map (func(a)(** a 2))
		(zip (func(a b)(* a b))
			p-ls1
			p-ls2
		)
	)

	)
)

(def val 5)
(def ls p-ls1)

(def _f(func(_min_idx _max_idx)
	(if (< (- _max_idx _min_idx) 2)
		-1
		{
			(def _m (to-int (/ (+ _max_idx _min_idx) 2)))
			(debug _m (elem-at ls _m))
			(if (< (elem-at ls _m) val)
				(_f _m _max_idx)
				(if (< val (elem-at ls _m))
					(_f _left _m)
					_m
					)
				)
		}
		)
	)
)