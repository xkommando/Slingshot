(def math-PI 3.14159265358979)
(def math-E 2.71828182845905)


(def even?
	(func(at)
		(== (% at 2) 0)
	)
)

(def odd?
	(func(at)
		(not (even? at))
	)
)


(def prime? 
	(func(a)
		(if(even? a)
			False
			{
				(def d 3)
				(def ret True)
				{	(while (< (* d d) a)
						{
							(if (eq? 0 (% a d))
								{
									(set! d a)
									(set! ret False)
								}
								(set! d (+ d 2))
							)
						}
					)
					ret
				}
			}
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