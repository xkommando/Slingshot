// 				Slingshot Standard Library
// 					version 0.3
// 				Copyright 2014  Cai Bowen


// alias for car
(def first
	(func(ls)
		(car ls)
	)
)

// alias for cdr
(def rest 
	(func(ls)
		(cdr ls)
	)
)

// last element in a list
(def last
	(func(ls)
		(if(null? ls)
			[]
			(elem-at ls (- (length ls) 1) )
		)
	)
)

// alias for null?
(def empty? 
	(func(ls)
		(null? ls)
	)
)

(def contains?
	(func (ls atom)
		(if (null? ls)
			False
			(if (eq? atom (car ls))
				True
				(contains? (cdr ls) atom)
			)
		)
	)
)


(def gen-list
	(func(ls f len)
		(if (== 0 len)
			ls
			{ // (log ls len)
				(gen-list (append ls (f ls) ) f (- len 1))
			}
		)
	)
)

(def first-index-of
	(func(ls idx atom)
		(if (null? ls)
			-1
			(if (eq? (car ls) atom)
				idx
				(index-of (cdr ls) (+ idx 1) atom)
			)
		)
	)
)
