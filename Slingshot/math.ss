(def math-PI 3.14159265358979)
(def math-E 2.71828182845905)


(def less?
	(func(a b)
		(< a b)
	)
)
(def greater?
	(func(a b)
		(> a b)
	)
)


(def even?
	(func(at)
		(if(integer? at)
			(== (% at 2) 0)
			False
		)
	)
)

(def odd?
	(func(at)
		(not (even? at))
	)
)


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
						)
					)
				ret
				}
			}
		)
	)
)

(def factorial
	(func(a)
		(if (or (== a 1)
				(not (integer? a))
			)
			1
			(* a (factorial (- a 1) ) )
		)
	)
)

(def cot 
	(func(a)
		(/ 1 (tan a))
	)
)

(def sec
	(func(a)
		(/ 1 (cos a))
	)
)

(def csc
	(func(a)
		(/ 1 (sin a))
	)
)