using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyRandomizerMod
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>( this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> selector, bool checkLengths = true, bool fillMissing = false )
        {
            if( first == null ) { throw new ArgumentNullException( "first" ); }
            if( second == null ) { throw new ArgumentNullException( "second" ); }
            if( selector == null ) { throw new ArgumentNullException( "selector" ); }

            using( IEnumerator<TFirst> e1 = first.GetEnumerator() )
            {
                using( IEnumerator<TSecond> e2 = second.GetEnumerator() )
                {
                    while( true )
                    {
                        bool more1 = e1.MoveNext();
                        bool more2 = e2.MoveNext();

                        if( !more1 || !more2 )
                        { //one finished
                            if( checkLengths && !fillMissing && ( more1 || more2 ) )
                            { //checking length && not filling in missing values && ones not finished
                                throw new Exception( "Enumerables have different lengths (" + ( more1 ? "first" : "second" ) + " is longer)" );
                            }

                            //fill in missing values with default(Tx) if asked too
                            if( fillMissing )
                            {
                                if( more1 )
                                {
                                    while( e1.MoveNext() )
                                    {
                                        yield return selector( e1.Current, default( TSecond ) );
                                    }
                                }
                                else
                                {
                                    while( e2.MoveNext() )
                                    {
                                        yield return selector( default( TFirst ), e2.Current );
                                    }
                                }
                            }

                            yield break;
                        }

                        yield return selector( e1.Current, e2.Current );
                    }
                }
            }
        }
    }
}
