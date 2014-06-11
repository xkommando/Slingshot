// 				Slingshot Standard Library
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

(def mul-list
	(func(ls)
		(if(null? ls)
			1
			(* (car ls) (mul-list (cdr ls)))
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
