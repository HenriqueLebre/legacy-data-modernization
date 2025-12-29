/*
 * Example.prg - Database Wrapper Usage Example
 */

#include "hbclass.ch"

FUNCTION Main()

   ? "=== Database Wrapper Example ==="
   ?
   
   // Initialize
   IF !DbWrapperInit( "http://localhost:5000" )
      ? "ERROR: Failed to connect to API"
      RETURN NIL
   ENDIF
   
   ? "Connected to API!"
   ?
   
   // Open table
   DbUseApi( "CUSTOMERS", "CLI", .T. )
   oWA := GetCurrentWorkArea()
   
   ? "Opened CUSTOMERS - " + Str( oWA:RecCount() ) + " records"
   ?
   
   // Navigate
   oWA:GoTop()
   DO WHILE !oWA:Eof()
      ? "  " + Str( oWA:RecNo() ) + ") " + FIELD( "CLI", "code" ) + " - " + FIELD( "CLI", "name" )
      oWA:Skip()
   ENDDO
   ?
   
   // Search
   ? "Seeking CLI-001..."
   IF oWA:Seek( "CLI-001" )
      ? "  Found: " + FIELD( "CLI", "name" )
   ELSE
      ? "  Not found"
   ENDIF
   ?
   
   // Close
   DbCloseApi( "CLI" )
   ? "Done!"
   
   RETURN NIL
