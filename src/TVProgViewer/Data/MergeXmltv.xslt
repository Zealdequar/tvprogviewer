<?xml version="1.0" encoding="utf-8"?>
<!-- Stylesheet: MergeSimple.xslt -->
<!-- Импортировать эту таблицу стилей в другую, где определен ключ -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" indent="yes"/>
    <xsl:template match="/*">
      <!-- Копировать самый внешний элемент исходного документа -->
      <xsl:copy>
        <!-- Для каждого дочернего элемента в исходном документе решаем, нужно ли копировать его в выходной, основываясь на том, есть он в другом документе или нет -->
        <xsl:for-each select="*">
          <!-- Вызываем шаблон, который определяет уникальный ключ для этого элемента. Он должен быть определен во включающей таблице стилей. -->
          <xsl:variable name="key-value1">
            <xsl:call-template name="mergekey-value1"/> 
          </xsl:variable> 
          <xsl:variable name="key-value2">
            <xsl:call-template name="mergekey-value2"/>
          </xsl:variable>
          <xsl:variable name="element" select="."/>
          <!-- Этот цикл for-each нужен просто для переключения контекста на второй документ. -->
          <xsl:for-each select="document('xmltv2.xml')/*">
            <!-- Применяем механизм ключей для проверки присутствия элемента во втором документе. Ключ должен быть определен во включающей таблице стилей. -->
             <xsl:if test="not(key('mergekey1', $key-value1)) and not(key('mergekey2', $key-value2))">
              <xsl:copy-of select="$element"/>
             </xsl:if>
          </xsl:for-each>
        </xsl:for-each>
        <!-- Копируем все элементы из второго документа. -->
        <xsl:copy-of select="document('xmltv2.xml')/*/*"/>
      </xsl:copy>
    </xsl:template>
</xsl:stylesheet>

