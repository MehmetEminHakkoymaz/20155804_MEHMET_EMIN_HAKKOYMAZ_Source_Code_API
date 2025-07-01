<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes"/>
  
  <xsl:template match="/">
    <html>
      <head>
        <title>Product Catalog</title>
        <style>
          table { border-collapse: collapse; width: 100%; }
          th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }
          th { background-color: #f2f2f2; }
          tr:hover { background-color: #f5f5f5; }
        </style>
      </head>
      <body>
        <h1>Product Catalog</h1>
        <table>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Description</th>
            <th>Price</th>
            <th>Stock</th>
          </tr>
          <xsl:for-each select="Products/Product">
            <tr>
              <td><xsl:value-of select="Id"/></td>
              <td><xsl:value-of select="Name"/></td>
              <td><xsl:value-of select="Description"/></td>
              <td>$<xsl:value-of select="Price"/></td>
              <td><xsl:value-of select="StockQuantity"/></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>