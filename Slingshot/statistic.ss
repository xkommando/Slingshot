// 				Slingshot Standard Library
//					Statistic library
// 					version 0.3
// 				Copyright 2014  Cai Bowen

(def sum
	(func(ls)
		(if(null? ls)
			0
			(+ (car ls) (sum (cdr ls)))
		)
	)
)

(def mul-list
	(func(ls)
		(if(null? ls)
			1
			(* (car ls) (mul-list (cdr ls)))
		)
	)
)


(def avg
	(func(ls)
		(if (null? ls)
			0
			{(def _total (sum ls))
			(def _count (length ls))
			(/ _total _count)
			}
			
		)
	)
)

// geometric average
(def geo-avg
	(func(ls)
		(if(null? ls)
			0
			{	(def _len (length ls))
			(def _mul-sum (mul-list ls))
			(** _mul-sum (/ 1 _len))
			}
		)
	)
)

// (def variance
// 	(func(ls)
// 		(if(null? ls)
// 			0
// 			{(def _avg (avg ls))
// 			(/ (sum (map (func(a)(** (- a _avg) 2))
// 						ls)
// 				)
// 				(length ls)
// 			)
// 			}
// 		)
// 	)
// )

// optimized
(require map avg length)
(def variance
	(func(ls)
		(if(null? ls)
			0
			{
				(def _avg (avg ls))
				(def _len (length ls))
				( / 	(- (sum (map (func(a)(** a 2)) ls) )
						(* _len (** _avg 2))
					)
				_len)
			}
		)
	)
)

(def std-deviation
	(func(ls)
		(if(null? ls)
			0
			{
				(def _avg (avg ls))
				(def _len (length ls))
				(def _prod (sum (map (func(a)(** a 2)) ls) ) )
				(**	
					(- (/ _prod _len)
						(** _avg 2) )
				0.5)
			}
		)
	)
)

// find the max/min value in a list
(def peak-of
	(func(ls compare)
	{
		(def _f(func(_at _ls)
				(if (null? _ls)
					_at
					(if(compare _at (car _ls))
						(_f _at (cdr _ls))
						(_f (car _ls) (cdr _ls))
					)
				)
			)
		)
		(if (null? ls)
			False
			(_f (car ls) (cdr ls))
		)
	}
	)
)

(def count
	(func(ls init pred?)
		(if (null? ls)
			init
			(if (pred? (car ls))
				(count (cdr ls) (+ init 1) pred?)
				(count (cdr ls) init pred?)
			)
		)
	)
)